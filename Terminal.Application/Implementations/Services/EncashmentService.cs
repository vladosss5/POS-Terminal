using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.DbEntities.MainDb;
using Terminal.Core.Enums;
using Terminal.Core.Models;
using Terminal.Persistence.EventDB;
using Terminal.Persistence.MainDB;
using Terminal.Persistence.TmsClient;

namespace Terminal.Application.Implementations.Services;

/// <inheritdoc/>
public class EncashmentService : IEncashmentService
{
    /// <inheritdoc cref="ILogger" />
    private readonly ILogger<EncashmentService> _logger;
    
    /// Фабрика экземпляров: <inheritdoc cref="DataContext"/>
    private readonly IDbContextFactory<DataContext> _dbFactory;

    /// Фабрика экземпляров: <inheritdoc cref="EventDbContext"/>
    private readonly IDbContextFactory<EventDbContext> _eventDbFactory;
    
    /// <inheritdoc cref="ICryptographyService" />
    private readonly ITmsClient _tmsClient;
    
    /// <inheritdoc cref="IParameterService" />
    private readonly IParameterService _parameterService;
    
    /// <inheritdoc cref="ICryptographyService" />
    private readonly ICryptographyService _cryptographyService;
    
    /// <inheritdoc cref="IConfigurationService" />
    private readonly IConfigurationService _configurationService;

    /// <summary>
    /// Отправляемые таблицы.
    /// </summary>
    private readonly List<TableToSendDto> _tablesToSend;
    
    /// <summary>
    /// Временная директория для файлов отправки в TMS.
    /// </summary>
    private readonly string _tempDirectory;
    
    /// <summary>
    /// Настройки сериализации.
    /// </summary>
    private static readonly JsonSerializerOptions JsonOptions = new() 
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public EncashmentService(
        IConfigurationService configurationService, 
        IDbContextFactory<DataContext> dbFactory, 
        IDbContextFactory<EventDbContext> eventDbFactory, 
        ILogger<EncashmentService> logger, 
        ITmsClient tmsClient, 
        IParameterService parameterService, 
        ICryptographyService cryptographyService)
    {
        _configurationService = configurationService;
        _dbFactory = dbFactory;
        _eventDbFactory = eventDbFactory;
        _logger = logger;
        _tmsClient = tmsClient;
        _parameterService = parameterService;
        _cryptographyService = cryptographyService;

        _tablesToSend = configurationService.GetTablesToSend();
        
        _tempDirectory = Path.Combine(Path.GetTempPath(), "TerminalEncashment");
        
        Directory.CreateDirectory(_tempDirectory);
    }
    
    /// <inheritdoc/>
    public async Task EncashmentAsync()
    {
        _logger.LogInformation($"Encashment start in {DateTime.Now:HH:mm:ss.ffffff}");

        await ProcessingResultLastEncashmentAsync();
        
        var dbMain = await _dbFactory.CreateDbContextAsync();
        var dbEvent = await _eventDbFactory.CreateDbContextAsync();
        
        foreach (var tableToSend in _tablesToSend)
        {
            switch (tableToSend.Name)
            {
                case EncashmentTable.Sales:
                    await EncashmentSellingTable(dbMain, tableToSend);
                    break;
            }
        }

        await _tmsClient.StartEncashmentOnTmsAsync();
        
        _logger.LogInformation($"Encashment end in {DateTime.Now:HH:mm:ss.ffffff}");
    }

    /// <summary>
    /// Обработка результатов прошлых инкассаций.
    /// </summary>
    private async Task ProcessingResultLastEncashmentAsync()
    {
        var terminalNumber = await _parameterService.GetValueAsync(AppParameter.SerialNO111);
        var archiveBytes = await _tmsClient.GetResultsEncashmentCollectionAsync(terminalNumber);

        if (archiveBytes.Length == 0)
            return;
        
        using var memoryStream = new MemoryStream(archiveBytes);
        await using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
        
        await using var entryStream = await archive.Entries.First().OpenAsync();
        var encashmentRows = await JsonSerializer.DeserializeAsync<List<EncashmentResultRowDto>>(entryStream, JsonOptions);
        
        await DeleteSuccessfulEncashmentsAsync(encashmentRows);
    }

    /// <summary>
    /// Удалить успешно инкассированные строки из БД. 
    /// </summary>
    /// <param name="encashmentRows"></param>
    private async Task DeleteSuccessfulEncashmentsAsync(IEnumerable<EncashmentResultRowDto>? encashmentRows)
    {
        if (encashmentRows == null)
            return;

        var rows = encashmentRows as EncashmentResultRowDto[] ?? encashmentRows.ToArray();
        var dbContext = await _dbFactory.CreateDbContextAsync();
        
        var salesToDelete = rows
            .Where(x => x is
            {
                TableName: EncashmentTable.Sales,
                Success: true
            })
            .Select(key => new Selling { TransactionShopKey = int.Parse(key.IdRowFromTable!) })
            .ToList();

        if (salesToDelete.Count != 0)
            dbContext.Sales.RemoveRange(salesToDelete);
        
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Инкассировать таблицу продаж.
    /// </summary>
    /// <param name="context">Контекст БД с таблицей продаж.</param>
    /// <param name="tableToSendDto">Параметр отправляемых таблиц.</param>
    private async Task EncashmentSellingTable(DataContext context, TableToSendDto tableToSendDto)
    {
        const int pageSize = 300;
        var lastTransactionShopKey = 0;
        var hasMore = true;
        
        while (hasMore)
        {
            var sales = await context.Sales
                .AsNoTracking()
                .OrderBy(x => x.TransactionShopKey)
                .Where(x => x.TransactionShopKey > lastTransactionShopKey)
                .Take(pageSize)
                .ToListAsync();

            var encashmentRows = sales
                .Select(x => new EncashmentRowDto
                {
                    TableName = EncashmentTable.Sales,
                    JsonData = JsonSerializer.Serialize(x, JsonOptions)
                }).ToList();
            
            if (encashmentRows.Count == 0)
                break;

            var batchNumber = lastTransactionShopKey / pageSize;
            var compressedFilePath = await SaveAndCompressBatchAsync(encashmentRows, tableToSendDto, batchNumber);

            if (compressedFilePath == string.Empty)
                return;
            
            var compressedData = await File.ReadAllBytesAsync(compressedFilePath);
            var fileName = Path.GetFileName(compressedFilePath);
            
            await _tmsClient.SendEncashmentTablesAsync(compressedData, tableToSendDto, fileName, encashmentRows.Count);
            
            File.Delete(compressedFilePath);
            
            lastTransactionShopKey = sales.Last().TransactionShopKey;
            hasMore = encashmentRows.Count == pageSize;
            
            encashmentRows.Clear();
            sales.Clear();
            
            if (batchNumber % 5 != 0) 
                continue;
            
            GC.Collect();
            await Task.Factory.StartNew(GC.WaitForPendingFinalizers, TaskCreationOptions.LongRunning);
        }
    }

    /// <summary>
    /// Сохранить и сжать пакет инкассируемых данных.
    /// </summary>
    /// <param name="encashmentRows">Список инкассируемых строк.</param>
    /// <param name="table">Отправляемая таблица.</param>
    /// <param name="batchNumber">Номер пакета в рамках инкассируемой таблицы.</param>
    /// <returns>Путь к сжатому файлу.</returns>
    private async Task<string> SaveAndCompressBatchAsync(List<EncashmentRowDto> encashmentRows, TableToSendDto table, int batchNumber)
    {
        var dateTimeNow = DateTime.UtcNow.ToString("HH.mm.ss.ff");
        
        var jsonFilePath = 
            Path.Combine(_tempDirectory, $"{table.Name}_batch_{batchNumber}_{dateTimeNow}.json");
        var compressedFilePath = 
            Path.Combine(_tempDirectory, $"{table.Name}_batch_{batchNumber}_{dateTimeNow}.json.gz");
        
        try
        {
            await using (var jsonStream = File.Create(jsonFilePath))
            await using (var writer = new StreamWriter(jsonStream, Encoding.UTF8, 8192, leaveOpen: true))
            {
                foreach (var json in encashmentRows.Select(item => JsonSerializer.Serialize(item, JsonOptions)))
                    await writer.WriteLineAsync(json);

                await writer.FlushAsync();
            }

            await using (var originalStream = File.OpenRead(jsonFilePath))
            await using (var compressedStream = File.Create(compressedFilePath))
            await using (var gzipStream = new GZipStream(compressedStream, CompressionLevel.Optimal))
            {
                await originalStream.CopyToAsync(gzipStream);
                await gzipStream.FlushAsync();
            }
        
            File.Delete(jsonFilePath);
        
            return compressedFilePath;
        }
        catch (Exception e)
        {
            if (File.Exists(jsonFilePath)) 
                File.Delete(jsonFilePath);
            
            if (File.Exists(compressedFilePath)) 
                File.Delete(compressedFilePath);
            
            _logger.LogError(e.Message);
            
            return string.Empty;
        }
    }
}