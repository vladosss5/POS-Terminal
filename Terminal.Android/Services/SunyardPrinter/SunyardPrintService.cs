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


/// <summary>
/// Реализация сервиса печати для терминалов Sunyard на платформе Android.
/// Обеспечивает подключение к системному сервису Sunyard, получение объекта принтера,
/// формирование и печать чеков с поддержкой текста, форматирования и отрезки бумаги.
/// </summary>
public class SunyardPrintService : Java.Lang.Object, IPrintService
{
    /// <summary>
    /// Фабрика "<inheritdoc cref="DataContext"/>"
    /// </summary>
    private readonly IDbContextFactory<DataContext> _dbFactory;
    
    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger<SunyardPrintService> _logger;
    
    /// <summary>
    /// Доступ к глобальной информации о среде приложения.
    /// </summary>
    private readonly Context _context;
    
    private SunyardPrintListener? _currentPrintListener;
    private IDeviceService? _deviceService;
    private IPrinter? _printer;
    private bool _isConnected;
    private readonly object _lock = new();
    private SunyardServiceConnection? _serviceConnection;

    /// <summary>
    /// Возвращает true, если сервис подключён к системному сервису Sunyard и принтер доступен.
    /// </summary>
    public bool IsConnected => _isConnected;

    /// <summary>
    /// Событие, возникающее при изменении состояния подключения к принтеру.
    /// </summary>
    public event EventHandler<bool>? ConnectionChanged;
    
    /// <summary>
    /// Событие, возникающее при ошибках в работе сервиса (например, потеря соединения).
    /// </summary>
    public event EventHandler<string>? ErrorOccurred;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public SunyardPrintService(
        Context context, 
        IDbContextFactory<DataContext> dbFactory, 
        ILogger<SunyardPrintService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbFactory = dbFactory;
        _logger = logger;
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public void Disconnect()
    {
        if (_isConnected && _serviceConnection != null)
        {
            try
            {
                _context.UnbindService(_serviceConnection);
                _logger.LogInformation("Закрыто соединение с принтером.");
            }
            catch
            {
                // ignored
            }

            _isConnected = false;
            _printer = null;
            _deviceService = null;
            _serviceConnection = null;
            ConnectionChanged?.Invoke(this, false);
        }
    }

    /// <inheritdoc/>
    public async Task<PrinterStatus> GetStatusAsync()
    {
        CheckConnection();
        return await Task.Run(() =>
        {
            int status = _printer!.Status;
            return MapStatus(status);
        });
    }

    /// <inheritdoc/>
    public async Task<PrintResult> PrintSalesReceiptAsync(SalesReceipt salesReceipt)
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
            AddCenteredText(salesReceipt.Header);
            _printer.FeedLine(1);
            
            await using var db = await _dbFactory.CreateDbContextAsync();

            _logger.LogInformation($"Поиск рессурса ID: {salesReceipt.Selling.ResourceCode}");
            
            var resourse = await db.ResourceCodes
                .FirstOrDefaultAsync(x => x.FuelCodeKey == salesReceipt.Selling.ResourceCode);

            if (resourse == null)
                _logger.LogError($"Рессурс ID: {resourse.FuelCodeKey} найден");
            
            AddLeftText($"{resourse.ResourceName} x{salesReceipt.Selling.Amount}");
            AddRightText($"{salesReceipt.Selling.BasePrice:C}");

            AddCenteredText("-------------------");
            AddRightText($"ИТОГО: {salesReceipt.Total:C}");

            if (!string.IsNullOrEmpty(salesReceipt.Footer))
            {
                _printer.FeedLine(1);
                AddCenteredText(salesReceipt.Footer);
            }

            _printer.FeedLine(3);
            if (salesReceipt.CutPaper)
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

    /// <summary>
    /// Проверяет состояние подключения к принтеру.
    /// </summary>
    /// <exception cref="InvalidOperationException">Выбрасывается, если принтер не подключён.</exception>
    private void CheckConnection()
    {
        if (_isConnected && _printer != null)
            return;
        
        _logger.LogError("Printer is not connected.");
        throw new InvalidOperationException("Printer is not connected.");
    }

    /// <summary>
    /// Добавляет текст с выравниванием по центру в очередь печати.
    /// </summary>
    /// <param name="text">Текст для печати.</param>
    private void AddCenteredText(string text)
    {
        var bundle = new Bundle();
        bundle.PutInt("font", IPrintConstant.IFontSize.Normal);
        bundle.PutInt("align", IPrintConstant.IAlign.Center);
        _printer!.AddText(bundle, text);
    }

    /// <summary>
    /// Добавляет текст с выравниванием по левому краю в очередь печати.
    /// </summary>
    /// <param name="text">Текст для печати.</param>
    private void AddLeftText(string text)
    {
        var bundle = new Bundle();
        bundle.PutInt("font", IPrintConstant.IFontSize.Small);
        bundle.PutInt("align", IPrintConstant.IAlign.Left);
        _printer!.AddText(bundle, text);
    }

    /// <summary>
    /// Добавляет текст с выравниванием по правому краю в очередь печати.
    /// </summary>
    /// <param name="text">Текст для печати.</param>
    private void AddRightText(string text)
    {
        var bundle = new Bundle();
        bundle.PutInt("font", IPrintConstant.IFontSize.Small);
        bundle.PutInt("align", IPrintConstant.IAlign.Right);
        _printer!.AddText(bundle, text);
    }

    /// <summary>
    /// Преобразует код статуса из SDK в перечисление PrinterStatus.
    /// </summary>
    /// <param name="code">Код статуса из SDK.</param>
    /// <returns>Соответствующий статус принтера.</returns>
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