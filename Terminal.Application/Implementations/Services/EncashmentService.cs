using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Models;
using System.IO.Compression;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Terminal.Data.EventDB;
using Terminal.Data.MainDB;

namespace Terminal.Application.Implementations.Services;

/// <summary>
/// Сервис инкассации - отвечает за обмен данными с TMS сервером
/// </summary>
public class EncashmentService : IEncashmentService
{
    private readonly EncashmentConfig _config;
    private readonly ITmsClient _tmsClient;
    private readonly ILogger _logger;
    private readonly string _connectionString;
    
    private readonly IDbContextFactory<DataContext> _dbFactory;

    private readonly IDbContextFactory<EventDbContext> _eventDbFactory;

    private readonly IConfigurationService _configurationService;
    
    /// <summary>
    /// Событие для отслеживания прогресса
    /// </summary>
    public event Func<string, Task>? ProgressUpdated;
    
    public EncashmentService(
        ITmsClient tmsClient,
        ILogger<EncashmentService> logger, 
        IDbContextFactory<DataContext> dbFactory,
        IDbContextFactory<EventDbContext> eventDbFactory,
        IConfigurationService configurationService)
    {
        _config = new EncashmentConfig();
        _tmsClient = tmsClient;
        _logger = logger;
        _dbFactory = dbFactory;
        _configurationService = configurationService;
        _eventDbFactory = eventDbFactory;
        _connectionString = $"Data Source={_config.DatabasePath}";
    }
    
    /// <summary>
    /// Выполнить инкассацию - полный цикл обмена с сервером
    /// Соответствует CSncProtocol::SendTransactions()
    /// </summary>
    public async Task<EncashmentResult> ExecuteEncashmentAsync(CancellationToken cancellationToken = default)
    {
        var result = new EncashmentResult
        {
            Success = true,
            Data = { StartDate = DateTime.Now }
        };
        
        try
        {
            Directory.CreateDirectory(_config.OutPath);
            Directory.CreateDirectory(_config.InPath);
            Directory.CreateDirectory(_config.UpdatePath);
            
            await OnProgressAsync("Подключение к серверу...");
            if (!await _tmsClient.ConnectAsync(cancellationToken))
            {
                result.Success = false;
                result.ErrorMessage = "Не удалось подключиться к серверу";
                return result;
            }
            
            await OnProgressAsync("Авторизация...");
            var auth = await _tmsClient.AuthorizeAsync(cancellationToken);
            result.Data.AuthSuccess = auth.Success;
            
            if (!auth.Success)
            {
                result.Success = false;
                result.ErrorMessage = auth.ErrorMessage ?? "Ошибка авторизации";
                return result;
            }
            
            await OnProgressAsync("Проверка обновлений...");
            var updates = await _tmsClient.ReceiveUpdatesAsync(cancellationToken);
            if (updates is { Success: true, SavedFiles.Count: > 0 })
            {
                var updateResult = await ApplyUpdatesAsync(updates.SavedFiles[0], cancellationToken);
                result.NeedRestart = updateResult;
            }
            
            // 4. Получение обновлений таблиц (справочников)
            await OnProgressAsync("Получение справочников...");
            var tables = await _tmsClient.ReceiveTablesAsync(cancellationToken);
            if (tables.Success && tables.SavedFiles.Count > 0)
            {
                await ApplyTablesAsync(tables.SavedFiles, cancellationToken);
            }
            
            // 5. Отправка данных по каждому типу таблиц
            var hasData = await SendAllTablesAsync(cancellationToken, result);
            
            // 6. Отправка файла конфигурации (если требуется)
            await SendConfigurationFileAsync(cancellationToken);
            
            // 7. Завершение сеанса
            await OnProgressAsync("Завершение сеанса...");
            await _tmsClient.EndDialogAsync(cancellationToken);
            
            result.HasData = hasData;
            result.Data.EndDate = DateTime.Now;
            
            if (!hasData)
                await OnProgressAsync("Нет данных для передачи");
            else
                await OnProgressAsync("Инкассация завершена");
            
            // 8. Печать отчета об инкассации (если есть принтер)
            await PrintIncassationReportAsync(result.Data, cancellationToken);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
            _logger.LogError(ex, "Encashment failed");
        }
        finally
        {
            await _tmsClient.DisconnectAsync();
        }
        
        return result;
    }
    
    /// <summary>
    /// Отправка всех таблиц с данными на сервер
    /// </summary>
    private async Task<bool> SendAllTablesAsync(CancellationToken cancellationToken, EncashmentResult result)
    {
        var hasData = false;
        var tablesToSend = _configurationService.GetTablesToSend();
        
        await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);
        await using var eventDb = await _eventDbFactory.CreateDbContextAsync(cancellationToken);
        
        foreach (var table in tablesToSend)
        {
            await OnProgressAsync($"Подготовка {table.DisplayName}...");

            List<Dictionary<string, object?>> unsentData = [];

            switch (table.DbName)
            {
                case "DataContext":
                    unsentData = await GetUnsentDataAsync(db, table.Name, table.KeyField, cancellationToken);
                    break;
                
                case "EventDb":
                    unsentData = await GetUnsentDataAsync(eventDb, table.Name, table.KeyField, cancellationToken);
                    break;
            }
            
            var incassItem = new IncassationItem
            {
                TableName = table.Name,
                TableKey = table.KeyField,
                IncassBefore = unsentData.Count,
                Message = table.DisplayName,
                DoNotPrintIfEmpty = table.DoNotPrintIfEmpty
            };
            
            if (unsentData.Count == 0)
            {
                result.Data.Items.Add(incassItem);
                continue;
            }
            
            hasData = true;
            
            await OnProgressAsync($"Отправка {table.DisplayName} ({unsentData.Count} записей)...");
            
            var zipData = await CreateDataArchiveAsync(table.Name, table.KeyField, unsentData, cancellationToken);
            var sendResult = await _tmsClient.SendTableAsync(table.Name, table.KeyField, zipData, cancellationToken);
            
            if (sendResult is { Success: true, ResponseData: not null })
                await ProcessServerResponseAsync(db, table.Name, table.KeyField, sendResult.ResponseData, cancellationToken);
            
            incassItem.IncassAfter = await GetUnsentCountAsync(db, table.Name, cancellationToken);
            incassItem.Success = sendResult.Success;
            
            result.Data.Items.Add(incassItem);
        }
        
        return hasData;
    }
    
    /// <summary>
    /// Получение непереданных записей из таблицы
    /// </summary>
    private async Task<List<Dictionary<string, object?>>> GetUnsentDataAsync(
        DbContext dbContext, 
        string tableName, 
        string keyField,
        CancellationToken cancellationToken)
    {
        var result = new List<Dictionary<string, object?>>();
        
        var sql = $"SELECT * FROM {tableName} WHERE IFNULL(ErrorCode, 0) = 0";

        await using var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = sql;
        
        if (command.Connection?.State != System.Data.ConnectionState.Open)
            await dbContext.Database.OpenConnectionAsync(cancellationToken);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        
        var schema = await reader.GetColumnSchemaAsync(cancellationToken);
        var columnNames = schema.Select(c => c.ColumnName).ToList();
        
        while (await reader.ReadAsync(cancellationToken))
        {
            var record = new Dictionary<string, object?>();
            foreach (var column in columnNames)
            {
                var value = reader[column];
                record[column] = value == DBNull.Value ? null : value;
            }
            result.Add(record);
        }
        
        return result;
    }
    
    /// <summary>
    /// Получение количества непереданных записей
    /// </summary>
    private async Task<int> GetUnsentCountAsync(
        DataContext dbContext,
        string tableName, 
        CancellationToken cancellationToken)
    {
        var sql = $"SELECT COUNT(*) FROM {tableName} WHERE IFNULL(ErrorCode, 0) = 0";

        await using var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = sql;
        
        if (command.Connection?.State != System.Data.ConnectionState.Open)
            await dbContext.Database.OpenConnectionAsync(cancellationToken);
        
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }
    
    /// <summary>
    /// Создание zip архива с данными для отправки
    /// </summary>
    private async Task<byte[]> CreateDataArchiveAsync(
        string tableName,
        string keyField,
        List<Dictionary<string, object?>> records,
        CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream();

        await using (var zip = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            var columns = records.Count > 0 
                ? string.Join(",", records[0].Keys) 
                : string.Empty;
            var infoContent = $"{tableName};{columns};{keyField}";
            var infoBytes = Encoding.UTF8.GetBytes(infoContent);
        
            var infoEntry = zip.CreateEntry("info");
            await using (var infoStream = await infoEntry.OpenAsync(cancellationToken))
            {
                await infoStream.WriteAsync(infoBytes, cancellationToken);
            }
        
            var dataContent = new StringBuilder();
            foreach (var record in records)
            {
                var values = record.Values.Select(v => FormatValueForExport(v));
                dataContent.AppendLine(string.Join("\t", values));
            }
            var dataBytes = Encoding.UTF8.GetBytes(dataContent.ToString());
        
            var dataEntry = zip.CreateEntry("data");
            await using (var dataStream = await dataEntry.OpenAsync(cancellationToken))
            {
                await dataStream.WriteAsync(dataBytes, cancellationToken);
            }
        }
    
        memoryStream.Position = 0;
        return memoryStream.ToArray();
    }
    
    /// <summary>
    /// Форматирование значения для экспорта
    /// </summary>
    private string FormatValueForExport(object? value)
    {
        if (value == null || value == DBNull.Value)
            return "\\N";
        
        return value switch
        {
            DateTime dt => dt.ToString("yyyy-MM-dd HH:mm:ss"),
            decimal d => d.ToString(System.Globalization.CultureInfo.InvariantCulture),
            double d => d.ToString(System.Globalization.CultureInfo.InvariantCulture),
            float f => f.ToString(System.Globalization.CultureInfo.InvariantCulture),
            bool b => b ? "1" : "0",
            byte[] bytes => Convert.ToBase64String(bytes),
            _ => value.ToString()?.Replace("\t", " ").Replace("\n", " ") ?? "\\N"
        };
    }
    
    /// <summary>
    /// Обработка ответа сервера после отправки таблицы
    /// </summary>
    private async Task ProcessServerResponseAsync(
        DataContext db,
        string tableName,
        string keyField,
        SendTableResponseData responseData,
        CancellationToken cancellationToken)
    {
        // Правильно: BeginTransactionAsync вызывается через db.Database
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            // Обновляем успешно отправленные записи (ErrorCode = 0)
            if (responseData.SuccessKeys.Count > 0)
            {
                var keys = string.Join(",", responseData.SuccessKeys);
                var sql = $"UPDATE {tableName} SET ErrorCode = 0 WHERE {keyField} IN ({keys})";
                await db.Database.ExecuteSqlRawAsync(sql, cancellationToken);
            }
            
            // Обновляем записи с ошибками (ErrorCode = 1)
            if (responseData.ErrorKeys.Count > 0)
            {
                var keys = string.Join(",", responseData.ErrorKeys);
                var sql = $"UPDATE {tableName} SET ErrorCode = 1 WHERE {keyField} IN ({keys})";
                await db.Database.ExecuteSqlRawAsync(sql, cancellationToken);
            }
            
            // Обновляем записи с ошибками сохранения (ErrorCode = 2)
            if (responseData.ErrorSaveKeys.Count > 0)
            {
                var keys = string.Join(",", responseData.ErrorSaveKeys);
                var sql = $"UPDATE {tableName} SET ErrorCode = 2 WHERE {keyField} IN ({keys})";
                await db.Database.ExecuteSqlRawAsync(sql, cancellationToken);
            }
            
            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation($"Processed server response for {tableName}: " +
                $"Success={responseData.SuccessKeys.Count}, " +
                $"Errors={responseData.ErrorKeys.Count}, " +
                $"ErrorSave={responseData.ErrorSaveKeys.Count}");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, $"Failed to process server response for {tableName}");
            throw;
        }
    }
    
    /// <summary>
    /// Применение обновлений ПО
    /// </summary>
    public async Task<bool> ApplyUpdatesAsync(string updatePath, CancellationToken cancellationToken = default)
    {
        try
        {
            await OnProgressAsync("Применение обновлений...");
            
            // TODO: Полная реализация требует:
            // 1. Распаковку zip архива с обновлением
            // 2. Проверку целостности и подписи
            // 3. Остановку сервисов
            // 4. Замену файлов
            // 5. Перезапуск приложения
            
            _logger.LogInformation($"Update received: {updatePath}");
            
            // Проверяем наличие файла обновления
            if (!File.Exists(updatePath))
            {
                _logger.LogWarning($"Update file not found: {updatePath}");
                return false;
            }
            
            // Распаковываем обновление во временную директорию
            var extractPath = Path.Combine(_config.UpdatePath, "temp_" + Guid.NewGuid());
            Directory.CreateDirectory(extractPath);
            
            ZipFile.ExtractToDirectory(updatePath, extractPath, true);
            
            // Проверяем наличие версии
            var versionFile = Path.Combine(extractPath, "terminal.info");
            if (File.Exists(versionFile))
            {
                var version = await File.ReadAllTextAsync(versionFile, cancellationToken);
                _logger.LogInformation($"Update version: {version}");
            }
            
            // TODO: Здесь должна быть логика применения обновления
            // В зависимости от содержимого update.zip
            
            // Очищаем временные файлы
            Directory.Delete(extractPath, true);
            File.Delete(updatePath);
            
            return true; // true если требуется перезагрузка
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply updates failed");
            return false;
        }
    }
    
    /// <summary>
    /// Применение полученных таблиц (справочников)
    /// </summary>
    public async Task<bool> ApplyTablesAsync(IEnumerable<string> tableFiles, CancellationToken cancellationToken = default)
    {
        try
        {
            await OnProgressAsync("Применение справочников...");
            
            await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);
            
            await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

            var tableFilesArray = tableFiles as string[] ?? tableFiles.ToArray();
            
            foreach (var file in tableFilesArray)
            {
                await OnProgressAsync($"Обработка {Path.GetFileName(file)}...");

                await using var zip = await ZipFile.OpenReadAsync(file, cancellationToken);
                
                var infoEntry = zip.GetEntry("info");
                if (infoEntry == null)
                {
                    _logger.LogWarning($"No info file in {file}");
                    continue;
                }

                await using var infoStream = await infoEntry.OpenAsync(cancellationToken);
                using var infoReader = new StreamReader(infoStream, Encoding.UTF8);
                var infoContent = await infoReader.ReadToEndAsync(cancellationToken);
                var infoParts = infoContent.Split(';');
                
                if (infoParts.Length < 3)
                {
                    _logger.LogWarning($"Invalid info format in {file}");
                    continue;
                }
                
                var tableName = infoParts[0];
                
                var dataEntry = zip.GetEntry("data");
                if (dataEntry == null)
                {
                    _logger.LogWarning($"No data file in {file}");
                    continue;
                }
                
                using var dataStream = dataEntry.Open();
                using var dataReader = new StreamReader(dataStream, Encoding.UTF8);
                var sqlContent = await dataReader.ReadToEndAsync(cancellationToken);
                var statements = SplitSqlStatements(sqlContent);
                
                // Очищаем таблицу перед вставкой
                await db.Database.ExecuteSqlRawAsync($"DELETE FROM {tableName}", cancellationToken);
                
                foreach (var statement in statements.Where(s => !string.IsNullOrWhiteSpace(s)))
                {
                    await db.Database.ExecuteSqlRawAsync(statement, cancellationToken);
                }
            }
            
            await transaction.CommitAsync(cancellationToken);
            
            foreach (var file in tableFilesArray)
            {
                try { File.Delete(file); } catch { }
            }
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply tables failed");
            return false;
        }
    }
    
    /// <summary>
    /// Разделение SQL скрипта на отдельные statements
    /// </summary>
    private List<string> SplitSqlStatements(string sql)
    {
        var statements = new List<string>();
        var currentStatement = new StringBuilder();
        var inString = false;
        var inQuote = false;
        
        foreach (var c in sql)
        {
            switch (c)
            {
                case '\'' when !inQuote:
                    inString = !inString;
                    break;
                case '"' when !inString:
                    inQuote = !inQuote;
                    break;
            }
            
            if (!inString && !inQuote && c == ';')
            {
                statements.Add(currentStatement.ToString().Trim());
                currentStatement.Clear();
                continue;
            }
            
            currentStatement.Append(c);
        }
        
        var lastStatement = currentStatement.ToString().Trim();
        if (!string.IsNullOrEmpty(lastStatement))
            statements.Add(lastStatement);
        
        return statements;
    }
    
    /// <summary>
    /// Отправка файла конфигурации на сервер
    /// </summary>
    private async Task SendConfigurationFileAsync(CancellationToken cancellationToken)
    {
        var configPath = Path.Combine(_config.OutPath, "config.zip");
        if (!File.Exists(configPath))
        {
            configPath = await CreateConfigurationZipAsync(cancellationToken);
        }
        
        if (File.Exists(configPath))
        {
            await _tmsClient.SendFileAsync(configPath, "config.zip", cancellationToken);
            File.Delete(configPath);
        }
    }
    
    /// <summary>
    /// Создание zip архива с конфигурацией
    /// Соответствует CSncProtocol::MakeConfigCopy()
    /// </summary>
    private async Task<string> CreateConfigurationZipAsync(CancellationToken cancellationToken)
    {
        var configPath = Path.Combine(_config.OutPath, $"config_{DateTime.Now:yyyyMMddHHmmss}.zip");
        Directory.CreateDirectory(_config.OutPath);

        await using var zip = await ZipFile.OpenAsync(configPath, ZipArchiveMode.Create, cancellationToken);
        
        // Добавляем файлы конфигурации
        var filesToAdd = new[]
        {
            ("terminal.db", "terminal.db"),
            ("terminal.info", "terminal.info"),
            ("config.xml", "config.xml"),
            ("menu.xml", "menu.xml"),
            ("params.xml", "params.xml"),
            ("chanels.xml", "chanels.xml"),
            ("schema.xml", "schema.xml"),
            ("limitation.xml", "limitation.xml")
        };
        
        foreach (var (source, dest) in filesToAdd)
        {
            var sourcePath = Path.Combine(AppContext.BaseDirectory, source);
            if (File.Exists(sourcePath))
            {
                await zip.CreateEntryFromFileAsync(sourcePath, dest, cancellationToken);
            }
        }
        
        // TODO: Добавить сертификат (cert.pem)
        
        return configPath;
    }
    
    /// <summary>
    /// Печать отчета об инкассации
    /// </summary>
    private async Task PrintIncassationReportAsync(IncassationData data, CancellationToken cancellationToken)
    {
        // TODO: Реализация печати отчета
        // Формирование чека инкассации аналогично CPrinterTaskIncassation
        _logger.LogInformation($"Incassation completed: Start={data.StartDate}, End={data.EndDate}, AuthSuccess={data.AuthSuccess}");
    }
    
    private async Task OnProgressAsync(string message)
    {
        _logger.LogInformation(message);
        if (ProgressUpdated != null)
            await ProgressUpdated.Invoke(message);
    }
}