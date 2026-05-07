using System.IO.Compression;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.DbEntitiesServices;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.DbEntities;
using Terminal.Core.Enums;
using Terminal.Core.Models;
using Terminal.Data.Context;

namespace Terminal.Application.Implementations.Services;

public class IncassationService : IIncassationService
{
    private readonly IDbContextFactory<DataContext> _dbFactory;
    private readonly ITmsConnectionService _tmsConnectionService;
    private readonly IParameterService _parameterService;
    private readonly IShiftService _shiftService;
    private readonly IConfigurationService _configurationService;
    private readonly ILogger<IncassationService> _logger;
    private readonly IFileExplorer _fileExplorer;
    
    
    public IncassationService(
        IDbContextFactory<DataContext> dbFactory,
        ITmsConnectionService tmsConnectionService,
        IShiftService shiftService,
        IConfigurationService configurationService,
        ILogger<IncassationService> logger,
        IFileExplorer fileExplorer, 
        IParameterService parameterService)
    {
        _dbFactory = dbFactory;
        _tmsConnectionService = tmsConnectionService;
        _shiftService = shiftService;
        _configurationService = configurationService;
        _logger = logger;
        _fileExplorer = fileExplorer;
        _parameterService = parameterService;
    }
    
    
    public async Task<IncassationData> CollectIncassationDataAsync(CancellationToken cancellationToken = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);
        
        var currentShift = await _shiftService.GetOpenedShiftOrDefaultAsync();
        var shiftKey = currentShift?.ShiftKey ?? 0;
        
        var prohibitionCount = await db.Prohibitions.CountAsync(cancellationToken);
        var allowCount = await db.Allows.CountAsync(cancellationToken);
        
        var updateCount = await db.CardUpdates
            .CountAsync(x => (x.IsSent == null || !x.IsSent.Value) && x.ResultCode != 4, cancellationToken);
        
        var aswCount = await db.Sales
            .CountAsync(x => x.ShiftKey == shiftKey, cancellationToken);
        
        var asCount = await db.Sales
            .CountAsync(x => x.ShiftKey == shiftKey && 
                             x.BaseType == BasePaymentType.NonCash && 
                             x.DerivedType == DerivedPaymentType.BankCard, cancellationToken);
        
        var aplCount = await db.Payments
            .CountAsync(x => x.ShiftKey == shiftKey && x.IsSent == null, cancellationToken);
        
        var acdCount = await db.CardUpdates
            .CountAsync(x => x.ShiftKey == shiftKey && (x.IsSent == null || !x.IsSent.Value), cancellationToken);
        
        var abeCount = await db.Shifts.CountAsync(cancellationToken);
        var aoCount = await db.Dispensers.CountAsync(cancellationToken);
        
        var baseDate = new DateTime(2010, 1, 1);
        var calculatedShiftKey = (int)(DateTime.Now.Date - baseDate).TotalDays;
        var finalShiftKey = shiftKey > 0 ? shiftKey : calculatedShiftKey;
        
        var incassationData = new IncassationData
        {
            ShiftKey = finalShiftKey,
            EventDate = DateTime.Now,
            ProhibitionCount = prohibitionCount,
            AllowCount = allowCount,
            UpdateCount = updateCount,
            AswCount = aswCount,
            AsCount = asCount,
            AplCount = aplCount,
            AcdCount = acdCount,
            AbeCount = abeCount,
            AoCount = aoCount
        };
        
        _logger.LogInformation(
            "Собраны данные инкассации: Shift={ShiftKey}, Продажи={AswCount}, Возвраты={AsCount}, " +
            "Платежи={AplCount}, Корректировки={AcdCount}",
            incassationData.ShiftKey, incassationData.AswCount, incassationData.AsCount,
            incassationData.AplCount, incassationData.AcdCount);
        
        return incassationData;
    }

    public async Task<IncassationResult> SendIncassationToTmsAsync(CancellationToken cancellationToken = default)
    {
        var result = new IncassationResult
        {
            IsSuccess = false,
            ProcessedAt = DateTime.Now
        };
        
        try
        {
            if (!_tmsConnectionService.IsConnected)
            {
                result.Message = "Нет подключения к TMS серверу";
                return result;
            }
            
            var incassationData = await CollectIncassationDataAsync(cancellationToken);
            
            var packetData = CreateIncassationPacket(incassationData);
            
            await _tmsConnectionService.SendDataAsync(packetData, cancellationToken);
            
            await SaveIncassationRecordAsync(incassationData, cancellationToken);
            
            result.IsSuccess = true;
            result.Data = incassationData;
            result.Message = $"Инкассация выполнена. Продажи: {incassationData.AswCount}, " +
                            $"Возвраты: {incassationData.AsCount}, Платежи: {incassationData.AplCount}";
            
            _logger.LogInformation(result.Message);
        }
        catch (Exception ex)
        {
            result.Message = $"Ошибка при отправке инкассации: {ex.Message}";
            _logger.LogError(ex, result.Message);
        }
        
        return result;
    }
    
    private byte[] CreateIncassationPacket(IncassationData data)
    {
        using var memoryStream = new MemoryStream();
        
        using (var zip = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            var entry = zip.CreateEntry("Ri.dbf");
            using var entryStream = entry.Open();
            using var writer = new StreamWriter(entryStream, Encoding.GetEncoding(866));
            
            writer.WriteLine("SHNUM,COD_AZS,DATINK,TIMINK,DTINK,PROH,EMIT,COR,ASW,AS,APL,ACD,ABE,AO");
            writer.WriteLine(
                $"{data.ShiftKey},1," +
                $"{data.EventDate:yyyyMMdd}," +
                $"{data.EventDate:HHmmss}," +
                $"{data.EventDate:yyyy-MM-dd HH:mm:ss}," +
                $"{data.ProhibitionCount}," +
                $"{data.AllowCount}," +
                $"{data.UpdateCount}," +
                $"{data.AswCount}," +
                $"{data.AsCount}," +
                $"{data.AplCount}," +
                $"{data.AcdCount}," +
                $"{data.AbeCount}," +
                $"{data.AoCount}");
        }
        
        return memoryStream.ToArray();
    }
    
    private async Task SaveIncassationRecordAsync(IncassationData data, CancellationToken cancellationToken)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);

        var terminalKey = int.Parse(await _parameterService.GetValueAsync(AppParameter.SerialNO111));
        
        var incassationEvent = new Event
        {
            TerminalKey = terminalKey,
            EventDate = DateTime.Now,
            EventType = (int)EventType.Incassation,
            EventObject = (int)EventObjects.Terminal,
            EventResult = (int)EventResult.Sent,
            EventInfo = $"Инкассация смены {data.ShiftKey}: Продажи={data.AswCount}, " +
                       $"Возвраты={data.AsCount}, Платежи={data.AplCount}"
        };
        
        await db.Events.AddAsync(incassationEvent, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }
    
    private async Task<long> GetTerminalKeyAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var setting = await db.Settings.FirstOrDefaultAsync(x => x.SettingsKey == SettingsKey.Sale);
        return setting?.Value ?? 1;
    }
}