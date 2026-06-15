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
    
    /// <summary>
    /// Кол-во инкассируемых записей в отправляемом пакете.
    /// </summary>
    private const int PageSize = 300;

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
        ITmsClient tmsClient)
    {
        _dbFactory = dbFactory;
        _eventDbFactory = eventDbFactory;
        _logger = logger;
        _tmsClient = tmsClient;

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
                    await EncashmentTableAsync<Selling>(dbMain, tableToSend,
                        q => q.OrderBy(x => x.TransactionShopKey),
                        x => x.TransactionShopKey,
                        nameof(Selling.TransactionShopKey));
                    break;
                
                case EncashmentTable.Shifts:
                    await EncashmentTableAsync<Shift>(dbMain, tableToSend,
                        q => q.OrderBy(x => x.ShiftShopKey),
                        x => x.ShiftShopKey,
                        nameof(Shift.ShiftShopKey));
                    break;
                
                case EncashmentTable.CardUpdates:
                    await EncashmentTableAsync<CardUpdate>(dbMain, tableToSend,
                        q => q.OrderBy(x => x.CardUpdateKey),
                        x => x.CardUpdateKey,
                        nameof(CardUpdate.CardUpdateKey));
                    break;
                
                case EncashmentTable.Repayments:
                    await EncashmentTableAsync<Repayment>(dbMain, tableToSend,
                        q => q.OrderBy(x => x.RepaymentShopKey),
                        x => x.RepaymentShopKey,
                        nameof(Repayment.RepaymentShopKey));
                    break;
                
                case EncashmentTable.Payments:
                    await EncashmentTableAsync<Payment>(dbMain, tableToSend,
                        q => q.OrderBy(x => x.PaymentShopKey),
                        x => x.PaymentShopKey,
                        nameof(Payment.PaymentShopKey));
                    break;
                
                case EncashmentTable.PosUpdates:
                    await EncashmentTableAsync<PosUpdate>(dbMain, tableToSend,
                        q => q.OrderBy(x => x.PosUpdateShopKey),
                        x => x.PosUpdateShopKey,
                        nameof(PosUpdate.PosUpdateShopKey));
                    break;
                
                case EncashmentTable.Dispensers:
                    await EncashmentTableAsync<Dispenser>(dbMain, tableToSend,
                        q => q.OrderBy(x => x.DispenserShopKey),
                        x => x.DispenserShopKey,
                        nameof(Dispenser.DispenserShopKey));
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
        var archiveBytes = await _tmsClient.GetResultsEncashmentCollectionAsync();

        if (archiveBytes.Length == 0)
            return;
        
        using var memoryStream = new MemoryStream(archiveBytes);
        await using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
        
        await using var entryStream = await archive.Entries.First().OpenAsync();
        var encashmentRows = await JsonSerializer.DeserializeAsync<List<EncashmentResultRowDto>>(entryStream, JsonOptions);
        
        if (encashmentRows == null)
            return;

        var dbContext = await _dbFactory.CreateDbContextAsync();
        
        var salesToDelete = encashmentRows
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
    /// Выполняет инкассацию таблицы с постраничной выгрузкой данных на сервер TMS.
    /// </summary>
    /// <typeparam name="T">Тип сущности БД.</typeparam>
    /// <param name="context">Контекст БД.</param>
    /// <param name="tableToSendDto">Метаданные отправляемой таблицы.</param>
    /// <param name="orderBy">Выражение сортировки (должно быть по возрастанию ключа).</param>
    /// <param name="keySelector">Функция получения ключа сущности для пагинации.</param>
    /// <param name="keyFieldName">Имя ключевого поля в БД.</param>
    private async Task EncashmentTableAsync<T>(
        DataContext context,
        TableToSendDto tableToSendDto,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
        Func<T, int> keySelector,
        string keyFieldName) where T : class
    {
        var lastKey = 0;
        var hasMore = true;

        while (hasMore)
        {
            var entities = await orderBy(context.Set<T>().AsNoTracking())
                .Where(x => EF.Property<int>(x, keyFieldName) > lastKey)
                .Take(PageSize)
                .ToListAsync();
            
            if (entities.Count == 0) break;
            
            var encashmentRows = entities
                .Select(x => new EncashmentRowDto
                {
                    TableName = tableToSendDto.Name,
                    JsonData = JsonSerializer.Serialize(x, JsonOptions)
                })
                .ToList();

            var batchNumber = lastKey / PageSize;
            var compressedFilePath = await SaveAndCompressBatchAsync(encashmentRows, tableToSendDto, batchNumber);

            if (compressedFilePath == string.Empty) return;

            var compressedData = await File.ReadAllBytesAsync(compressedFilePath);
            var fileName = Path.GetFileName(compressedFilePath);

            await _tmsClient.SendEncashmentTablesAsync(compressedData, tableToSendDto, fileName, encashmentRows.Count);

            File.Delete(compressedFilePath);

            lastKey = keySelector(entities.Last());
            hasMore = encashmentRows.Count == PageSize;

            encashmentRows.Clear();
            entities.Clear();

            if (batchNumber % 5 != 0) continue;

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
        var jsonFilePath = Path.Combine(_tempDirectory, $"{table.Name}_batch_{batchNumber}_{dateTimeNow}.json");
        var compressedFilePath = Path.Combine(_tempDirectory, $"{table.Name}_batch_{batchNumber}_{dateTimeNow}.json.gz");
        
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