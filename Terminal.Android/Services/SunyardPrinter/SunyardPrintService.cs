using System;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Com.Sunyard.Api;
using Com.Sunyard.Api.Printer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Enums;
using Terminal.Core.Models;
using Terminal.Data.Context;

namespace Terminal.Android.Services.SunyardPrinter;

public class SunyardPrintService : Java.Lang.Object, IPrintService
{
    private readonly IDbContextFactory<DataContext> _dbFactory;
    private readonly ILogger<SunyardPrintService> _logger;
    private readonly Context _context;
    
    private SunyardPrintListener? _currentPrintListener;
    private IDeviceService? _deviceService;
    private IPrinter? _printer;
    private bool _isConnected;
    private readonly object _lock = new();
    private SunyardServiceConnection? _serviceConnection;

    public bool IsConnected => _isConnected;

    public event EventHandler<bool>? ConnectionChanged;
    public event EventHandler<string>? ErrorOccurred;

    public SunyardPrintService(
        Context context, 
        IDbContextFactory<DataContext> dbFactory, 
        ILogger<SunyardPrintService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbFactory = dbFactory;
        _logger = logger;
    }

    public async Task<bool> ConnectAsync()
    {
        if (_isConnected) return true;

        var tcs = new TaskCompletionSource<bool>();

        _serviceConnection = new SunyardServiceConnection(
            onConnected: deviceService =>
            {
                lock (_lock)
                {
                    _deviceService = deviceService;
                    try
                    {
                        var binder = _deviceService.Printer;
                        _printer = IPrinter.Stub.AsInterface(binder);
                        _isConnected = true;
                        ConnectionChanged?.Invoke(this, true);
                        tcs.TrySetResult(true);
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(new Exception("Failed to get printer object", ex));
                    }
                }
            },
            onDisconnected: () =>
            {
                lock (_lock)
                {
                    _isConnected = false;
                    _printer?.Dispose();
                    _printer = null;
                    _deviceService?.Dispose();
                    _deviceService = null;
                    ConnectionChanged?.Invoke(this, false);
                }
            });

        var intent = new Intent("com.sunyard.api.device_service");
        intent.SetPackage("com.sunyard.deviceservice");
        if (!_context.BindService(intent, _serviceConnection, Bind.AutoCreate))
        {
            tcs.TrySetException(new InvalidOperationException("Failed to bind to Sunyard service. Make sure the service is installed."));
            return false;
        }

        var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(10000));
        if (completedTask != tcs.Task)
        {
            if (_serviceConnection != null)
                _context.UnbindService(_serviceConnection);
            throw new TimeoutException("Connection to Sunyard service timed out.");
        }

        return await tcs.Task;
    }

    public void Disconnect()
    {
        if (_isConnected && _serviceConnection != null)
        {
            try
            {
                _context.UnbindService(_serviceConnection);
            }
            catch { }
            _isConnected = false;
            _printer = null;
            _deviceService = null;
            _serviceConnection = null;
            ConnectionChanged?.Invoke(this, false);
        }
    }

    public async Task<PrinterStatus> GetStatusAsync()
    {
        CheckConnection();
        return await Task.Run(() =>
        {
            int status = _printer!.Status;
            return MapStatus(status);
        });
    }

    public async Task<PrintResult> PrintReceiptAsync(Receipt receipt)
    {
        CheckConnection();
        
        var status = await GetStatusAsync();
        if (status != PrinterStatus.Ready)
        {
            _logger.LogWarning($"Printer not ready: {status}");
            return new PrintResult { Success = false, ErrorMessage = $"Printer not ready: {status}" };
        }

        var tcs = new TaskCompletionSource<PrintResult>();

        try
        {
            _printer!.SetGray(5);
            AddCenteredText(receipt.Header);
            _printer.FeedLine(1);
            
            await using var db = await _dbFactory.CreateDbContextAsync();

            _logger.LogInformation($"Поиск рессурса ID: {receipt.Selling.ResourceCode}");
            
            var resourse = await db.ResourceCodes
                .FirstOrDefaultAsync(x => x.FuelCodeKey == receipt.Selling.ResourceCode);

            if (resourse == null)
                _logger.LogError($"Рессурс ID: {resourse.FuelCodeKey} найден");
            
            AddLeftText($"{resourse.ResourceName} x{receipt.Selling.Amount}");
            AddRightText($"{receipt.Selling.BasePrice:C}");

            AddCenteredText("-------------------");
            AddRightText($"ИТОГО: {receipt.Total:C}");

            if (!string.IsNullOrEmpty(receipt.Footer))
            {
                _printer.FeedLine(1);
                AddCenteredText(receipt.Footer);
            }

            _printer.FeedLine(3);
            if (receipt.CutPaper)
                _printer.CutPaper();

            _logger.LogInformation($"Чек составлен. Старт печати");
            _currentPrintListener = new SunyardPrintListener(tcs, _logger);
            _printer.StartPrint(_currentPrintListener);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка: {ex.Message}, {ex.StackTrace}");
            tcs.TrySetException(ex);
        }
        _logger.LogInformation($"Чек отпечатан");
        return await tcs.Task;
    }

    private void CheckConnection()
    {
        if (_isConnected && _printer != null)
            return;
        
        _logger.LogError("Printer is not connected.");
        throw new InvalidOperationException("Printer is not connected.");
    }

    private void AddCenteredText(string text)
    {
        var bundle = new Bundle();
        bundle.PutInt("font", IPrintConstant.IFontSize.Normal);
        bundle.PutInt("align", IPrintConstant.IAlign.Center);
        _printer!.AddText(bundle, text);
    }

    private void AddLeftText(string text)
    {
        var bundle = new Bundle();
        bundle.PutInt("font", IPrintConstant.IFontSize.Small);
        bundle.PutInt("align", IPrintConstant.IAlign.Left);
        _printer!.AddText(bundle, text);
    }

    private void AddRightText(string text)
    {
        var bundle = new Bundle();
        bundle.PutInt("font", IPrintConstant.IFontSize.Small);
        bundle.PutInt("align", IPrintConstant.IAlign.Right);
        _printer!.AddText(bundle, text);
    }

    private PrinterStatus MapStatus(int code)
    {
        return code switch
        {
            IPrintConstant.IErrorCode.ErrorNone => PrinterStatus.Ready,
            IPrintConstant.IErrorCode.ErrorPaperending => PrinterStatus.PaperEnded,
            IPrintConstant.IErrorCode.ErrorHarderr => PrinterStatus.HardwareError,
            IPrintConstant.IErrorCode.ErrorOverheat => PrinterStatus.Overheat,
            IPrintConstant.IErrorCode.ErrorBufoverflow=> PrinterStatus.BufferOverflow,
            IPrintConstant.IErrorCode.ErrorLowvol => PrinterStatus.LowVoltage,
            IPrintConstant.IErrorCode.ErrorPaperjam => PrinterStatus.PaperJam,
            IPrintConstant.IErrorCode.ErrorBusy => PrinterStatus.Busy,
            _ => PrinterStatus.Unknown
        };
    }
}