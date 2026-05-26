using System.IO.Compression;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.DbEntitiesServices;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.DbEntities.MainDb;
using Terminal.Core.Enums;
using Terminal.Core.Models;
using Terminal.Persistence.EventDB;
using Terminal.Persistence.MainDB;
using Terminal.Persistence.TmsClient;

namespace Terminal.Application.Implementations.Services;

public class EncashmentService : IEncashmentService
{
    /// Фабрика экземпляров: <inheritdoc cref="DataContext"/>
    private readonly IDbContextFactory<DataContext> _dbFactory;

    private readonly IDbContextFactory<EventDbContext> _eventDbFactory;
    
    private readonly ITmsClient _tmsClient;
    
    /// <inheritdoc cref="IParameterService" />
    private readonly IParameterService _parameterService;
    
    /// <inheritdoc cref="ICryptographyService" />
    private readonly ICryptographyService _cryptographyService;
    
    private readonly IConfigurationService _configurationService;
    
    private readonly ILogger<EncashmentService> _logger;

    private readonly List<TableToSendDto> _tablesToSend;
    
    private readonly string _tempDirectory;
    
    private readonly string _failedDirectory;
    
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = false };
    
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
        _failedDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "failed_encashment");
        
        Directory.CreateDirectory(_tempDirectory);
        Directory.CreateDirectory(_failedDirectory);
    }
    
    public async Task EncashmentAsync()
    {
        _logger.LogInformation($"Encashment start in {DateTime.Now.ToString("HH:mm:ss.ffffff")}");
        // TODO: Запрос конфигураций у TMS
        // TODO: Проверка закрытости смен
        
        await AuthenticationTmsClientAsync();
        
        var dbMain = await _dbFactory.CreateDbContextAsync();
        var dbEvent = await _eventDbFactory.CreateDbContextAsync();
        foreach (var tableToSend in _tablesToSend)
        {
            switch (tableToSend.Name)
            {
                case "selling":
                    await EncashmentSellingTable(dbMain, tableToSend);
                    break;
            }
        }
        
        _logger.LogInformation($"Encashment end in {DateTime.Now:HH:mm:ss.ffffff}");
    }

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
            
            if (sales.Count == 0)
                break;

            var batchNumber = lastTransactionShopKey / pageSize;
            var compressedFilePath = await SaveAndCompressBatchAsync(sales, tableToSendDto, batchNumber);
            
            var compressedData = await File.ReadAllBytesAsync(compressedFilePath);
            var transferSuccess = await _tmsClient.SendEncashmentTablesAsync(compressedData, tableToSendDto, batchNumber, sales.Count);
            
            if (!transferSuccess)
                File.Move(compressedFilePath, _failedDirectory);
            
            File.Delete(compressedFilePath);
            
            lastTransactionShopKey = sales.Last().TransactionShopKey;
            hasMore = sales.Count == pageSize;
            
            sales.Clear();
            
            if (batchNumber % 5 != 0) 
                continue;
            
            GC.Collect();
            await Task.Factory.StartNew(GC.WaitForPendingFinalizers, TaskCreationOptions.LongRunning);
        }
    }

    private async Task<string> SaveAndCompressBatchAsync<T>(List<T> data, TableToSendDto table, int batchNumber)
    {
        var jsonFilePath = Path.Combine(_tempDirectory, $"{table.Name}_batch_{batchNumber}.json");
        var compressedFilePath = Path.Combine(_tempDirectory, $"{table.Name}_batch_{batchNumber}.json.gz");
        
        try
        {
            await using (var jsonStream = File.Create(jsonFilePath))
            await using (var writer = new StreamWriter(jsonStream, Encoding.UTF8, 8192, leaveOpen: true))
            {
                foreach (var item in data)
                {
                    var json = JsonSerializer.Serialize(item, JsonOptions);
                    await writer.WriteLineAsync(json);
                }

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
            
            throw;
        }
    }
    
    private async Task AuthenticationTmsClientAsync()
    {
        if (_tmsClient.ConnectionStatus != TmsConnectionStatus.Authorized)
            return;
        
        var terminalNumber = await _parameterService.GetValueAsync(AppParameter.SerialNO111);
        var plainText = terminalNumber + " " + Guid.NewGuid();
        
        var password = _configurationService.CurrentSetting.TmsConfiguration!.Key;
        var salt = _configurationService.CurrentSetting.TmsConfiguration!.Salt;
        
        var workload = _cryptographyService.EncryptAes(plainText, password, Encoding.UTF8.GetBytes(salt));

        await _tmsClient.AuthenticationAsync(workload);
    }
}