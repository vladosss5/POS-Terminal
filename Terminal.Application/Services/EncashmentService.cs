using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Entities.DbEntities.MainDb;
using Terminal.Core.Entities.Models;
using Terminal.Core.Enums;
using Terminal.Core.Interfaces;
using Terminal.Core.IRepositories;

namespace Terminal.Application.Services;

/// <inheritdoc/>
public class EncashmentService : IEncashmentService
{
    /// <inheritdoc cref="ILogger" />
    private readonly ILogger<EncashmentService> _logger;

    /// <inheritdoc cref="IGenericRepository" />
    private readonly IGenericRepository _genericRepository;
    
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
    /// Кортеж с названиями таблицы и скриптами удаления записей по Id.
    /// </summary>
    private static readonly (EncashmentTable Table, string Sql)[] Deletions =
    [
        (EncashmentTable.Sales, "DELETE FROM Selling WHERE TransactionShopKey IN ({0})"),
        (EncashmentTable.Shifts, "DELETE FROM Shift WHERE ShiftShopKey IN ({0})"),
        (EncashmentTable.CardUpdates, "DELETE FROM card_update WHERE CardUpdateKey IN ({0})"),
        (EncashmentTable.Repayments, "DELETE FROM repayment WHERE RepaymentShopKey IN ({0})"),
        (EncashmentTable.Payments, "DELETE FROM payment WHERE PaymentShopKey IN ({0})"),
        (EncashmentTable.PosUpdates, "DELETE FROM pos_update WHERE PosUpdateShopKey IN ({0})"),
        (EncashmentTable.Dispensers, "DELETE FROM dispenser WHERE DispenserShopKey IN ({0})")
    ];
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public EncashmentService(
        IConfigurationService configurationService,
        ILogger<EncashmentService> logger, 
        ITmsClient tmsClient, 
        IGenericRepository genericRepository)
    {
        _logger = logger;
        _tmsClient = tmsClient;
        _genericRepository = genericRepository;

        _tablesToSend = configurationService.GetTablesToSend();
        
        _tempDirectory = Path.Combine(Path.GetTempPath(), "TerminalEncashment");
        
        Directory.CreateDirectory(_tempDirectory);
    }
    
    /// <inheritdoc/>
    public async Task EncashmentAsync()
    {
        _logger.LogInformation($"Encashment start in {DateTime.Now:HH:mm:ss.ffffff}");

        await ProcessingResultLastEncashmentAsync();
        
        foreach (var tableToSend in _tablesToSend)
        {
            switch (tableToSend.Name)
            {
                case EncashmentTable.Sales:
                {
                    var sales = await _genericRepository.GetALotOfStringFromArbitraryTableAsync<Selling>(
                        q => q.OrderBy(x => x.TransactionShopKey),
                        nameof(Selling.TransactionShopKey),
                        tableToSend.DbName);
                        
                    await EncashmentTableAsync(sales, tableToSend, x => x.TransactionShopKey);
                    break;
                }
                case EncashmentTable.Shifts:
                {
                    var shifts = await _genericRepository.GetALotOfStringFromArbitraryTableAsync<Shift>(
                        q => q.OrderBy(x => x.ShiftShopKey),
                        nameof(Shift.ShiftShopKey),
                        tableToSend.DbName);

                    await EncashmentTableAsync(shifts, tableToSend, x => x.ShiftShopKey);
                    break;
                }
                case EncashmentTable.CardUpdates:
                {
                    var cardUpdates = await _genericRepository.GetALotOfStringFromArbitraryTableAsync<CardUpdate>(
                        q => q.OrderBy(x => x.CardUpdateKey),
                        nameof(CardUpdate.CardUpdateKey),
                        tableToSend.DbName);

                    await EncashmentTableAsync(cardUpdates, tableToSend, x => x.CardUpdateKey);
                    break;
                }
                case EncashmentTable.Repayments:
                {
                    var repayments = await _genericRepository.GetALotOfStringFromArbitraryTableAsync<Repayment>(
                        q => q.OrderBy(x => x.RepaymentShopKey),
                        nameof(Repayment.RepaymentShopKey),
                        tableToSend.DbName);

                    await EncashmentTableAsync(repayments, tableToSend, x => x.RepaymentShopKey);
                    break;
                }
                case EncashmentTable.Payments:
                {
                    var payments = await _genericRepository.GetALotOfStringFromArbitraryTableAsync<Payment>(
                        q => q.OrderBy(x => x.PaymentShopKey),
                        nameof(Payment.PaymentShopKey),
                        tableToSend.DbName);

                    await EncashmentTableAsync(payments, tableToSend, x => x.PaymentShopKey);
                    break;
                }
                case EncashmentTable.PosUpdates:
                {
                    var posUpdates = await _genericRepository.GetALotOfStringFromArbitraryTableAsync<PosUpdate>(
                        q => q.OrderBy(x => x.PosUpdateShopKey),
                        nameof(PosUpdate.PosUpdateShopKey),
                        tableToSend.DbName);

                    await EncashmentTableAsync(posUpdates, tableToSend, x => x.PosUpdateShopKey);
                    break;
                }
                case EncashmentTable.Dispensers:
                {
                    var dispensers = await _genericRepository.GetALotOfStringFromArbitraryTableAsync<Dispenser>(
                        q => q.OrderBy(x => x.DispenserShopKey),
                        nameof(Dispenser.DispenserShopKey),
                        tableToSend.DbName);

                    await EncashmentTableAsync(dispensers, tableToSend, x => x.DispenserShopKey);
                    break;
                }
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

        if (archiveBytes.Length == 0) return;
        
        using var memoryStream = new MemoryStream(archiveBytes);
        await using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
        
        await using var entryStream = await archive.Entries.First().OpenAsync();
        var encashmentRows = await JsonSerializer.DeserializeAsync<List<EncashmentResultRowDto>>(entryStream, JsonOptions);
        
        if (encashmentRows == null) return;
        
        foreach (var (table, sql) in Deletions)
        {
            var ids = encashmentRows
                .Where(x => x.TableName == table && x.Success)
                .Select(x => x.IdEntityFromTable)
                .Select(int.Parse)
                .ToArray();
        
            if (ids.Length == 0) continue;

            var affected = await _genericRepository.ExecuteSqlAsync(string.Format(sql, string.Join(",", ids)), "DataContext");
        
            _logger.LogInformation("Deleted {Count} {Table} records", affected, table);
        }
    }

    /// <summary>
    /// Выполняет инкассацию таблицы с постраничной выгрузкой данных на сервер TMS.
    /// </summary>
    /// <typeparam name="T">Тип сущности в БД.</typeparam>
    /// <param name="entities">Коллекция отправляемых записей.</param>
    /// <param name="tableToSendDto">Метаданные отправляемой таблицы.</param>
    /// <param name="keySelector">Функция получения ключа сущности для пагинации.</param>
    private async Task EncashmentTableAsync<T>(
        List<T> entities,
        TableToSendDto tableToSendDto,
        Func<T, int> keySelector) where T : class
    {
        var lastKey = 0;
        var hasMore = true;

        while (hasMore)
        {
            if (entities.Count == 0) break;
            
            var encashmentRows = entities
                .Select(x => new EncashmentRowDto
                {
                    IdEntityFromTable = keySelector(x).ToString(),
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