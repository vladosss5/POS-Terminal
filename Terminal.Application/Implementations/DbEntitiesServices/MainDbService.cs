using System.IO.Compression;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Terminal.Application.Interfaces.DbEntitiesServices;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Enums;
using Terminal.Core.Models;
using Terminal.Persistence.MainDB;
using Terminal.Persistence.TmsClient;

namespace Terminal.Application.Implementations.DbEntitiesServices;

public class MainDbService : IMainDbService
{
    private readonly IConfigurationService _configurationService;

    private readonly ITmsClient _tmsClient;
    
    /// <inheritdoc cref="IParameterService" />
    private readonly IParameterService _parameterService;
    
    /// <inheritdoc cref="ICryptographyService" />
    private readonly ICryptographyService _cryptographyService;
    
    private readonly string _tempDirectory;
    
    private readonly string _failedDirectory;
    
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = false };

    public MainDbService(
        IConfigurationService configurationService, 
        ITmsClient tmsClient, 
        IParameterService parameterService, 
        ICryptographyService cryptographyService)
    {
        _configurationService = configurationService;
        _tmsClient = tmsClient;
        _parameterService = parameterService;
        _cryptographyService = cryptographyService;
        
        _tempDirectory = Path.Combine(Path.GetTempPath(), "TerminalEncashment");
        _failedDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "failed_encashment");
        
        Directory.CreateDirectory(_tempDirectory);
        Directory.CreateDirectory(_failedDirectory);
    }
    
    public async Task ExportDataFromMainDbAsync(TableToSendDto tableToSend, DataContext context)
    {
        if (tableToSend.DbName != nameof(DataContext))
            return;

        switch (tableToSend.Name)
        {
            case "selling":
                await ExportTableAsync(context.Sales, tableToSend, s => s.TransactionShopKey);
                break;
            
            case "shift":
                await ExportTableAsync(context.Shifts, tableToSend, s => s.ShiftShopKey);
                break;
            
            case "card_update":
                await ExportTableAsync(context.CardUpdates, tableToSend, c => c.CardUpdateKey);
                break;
            
            case "repayment":
                await ExportTableAsync(context.Repayments, tableToSend, r => r.RepaymentShopKey);
                break;
            
            case "payment":
                if (!tableToSend.DoNotPrintIfEmpty || await context.Payments.AnyAsync())
                    await ExportTableAsync(context.Payments, tableToSend, p => p.PaymentShopKey);
                break;
            
            case "pos_update":
                if (!tableToSend.DoNotPrintIfEmpty || await context.PosUpdates.AnyAsync())
                    await ExportTableAsync(context.PosUpdates, tableToSend, p => p.PosUpdateShopKey);
                break;
            
            case "dispenser":
                if (!tableToSend.DoNotPrintIfEmpty || await context.Dispensers.AnyAsync())
                    await ExportTableAsync(context.Dispensers, tableToSend, d => d.DispenserShopKey);
                break;
            
            default:
                Console.WriteLine($"Unknown table: {tableToSend.Name}");
                break;
        }
    }

    private async Task ExportTableAsync<T>(
        DbSet<T> dbSet, 
        TableToSendDto table, 
        Expression<Func<T, int>> keySelector) 
        where T : class
    {
        const int batchSize = 300;
        var lastKey = 0;
        var hasMore = true;
        var batchNumber = 0;

        var keyFunc = keySelector.Compile();

        while (hasMore)
        {
            var parameter = keySelector.Parameters[0];
            var body = Expression.GreaterThan(keySelector.Body, Expression.Constant(lastKey));
            var whereExpression = Expression.Lambda<Func<T, bool>>(body, parameter);
        
            var batch = await dbSet
                .AsNoTracking()
                .Where(whereExpression)
                .OrderBy(keySelector)
                .Take(batchSize)
                .ToListAsync();
            
            if (batch.Count == 0)
            {
                hasMore = false;
                continue;
            }
            
            lastKey = keyFunc(batch.Last());
            batchNumber++;

            var (compressedFilePath, originalSize, compressedSize) = 
                await SaveAndCompressBatchAsync(batch, table, batchNumber);

            await AuthenticationTmsClientAsync();
            var compressedData = await File.ReadAllBytesAsync(compressedFilePath);
            await _tmsClient.SendEncashmentTablesAsync(compressedData, table, batchNumber, batch.Count, originalSize, compressedSize);
            
            batch.Clear();

            if (batchNumber % 10 != 0) 
                continue;
            
            GC.Collect();
            await Task.Delay(50);
        }
    }
    
    private async Task<(string CompressedFilePath, long OriginalSize, long CompressedSize)> SaveAndCompressBatchAsync
        <T>(List<T> data, TableToSendDto table, int batchNumber)
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

            var originalSize = new FileInfo(jsonFilePath).Length;

            await using (var originalStream = File.OpenRead(jsonFilePath))
            await using (var compressedStream = File.Create(compressedFilePath))
            await using (var gzipStream = new GZipStream(compressedStream, CompressionLevel.Optimal))
            {
                await originalStream.CopyToAsync(gzipStream);
                await gzipStream.FlushAsync();
            }

            var compressedSize = new FileInfo(compressedFilePath).Length;
        
            File.Delete(jsonFilePath);
        
            return (compressedFilePath, originalSize, compressedSize);
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