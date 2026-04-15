using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Com.Sunyard.Api;
using Com.Sunyard.Api.Printer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Terminal.Application.Implementations.Services;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.DbEntities;
using Terminal.Core.Enums;
using Terminal.Core.Models;
using Terminal.Services.NavigationService;
using Terminal.ViewModels;
using Terminal.ViewModels.Pages;

namespace Terminal.Android.Services.Sunyard.SunyardPrinter;


/// <summary>
/// Реализация сервиса печати для терминалов Sunyard на платформе Android.
/// Обеспечивает подключение к системному сервису Sunyard, получение объекта принтера,
/// формирование и печать чеков с поддержкой текста, форматирования и отрезки бумаги.
/// </summary>
public class SunyardPrintService : Java.Lang.Object, IReceiptPrintService
{
    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger<SunyardPrintService> _logger;
    
    /// <summary>
    /// Доступ к глобальной информации о среде приложения.
    /// </summary>
    private readonly Context _context;

    /// <inheritdoc cref="INavigationService" />
    private readonly INavigationService _navigationService;
    
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
        ILogger<SunyardPrintService> logger, 
        INavigationService navigationService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger;
        _navigationService = navigationService;
    }
    
    /// <inheritdoc/>
    public async Task<PrintResult> PrintSalesReceiptAsync(SalesReceipt salesReceipt)
    {
        await ConnectAsync();
        CheckConnection();
        
        var status = await GetStatusAsync();
        if (status != PrinterStatus.Ready)
        {
            _logger.LogWarning($"Printer not ready: {status}");
            return new PrintResult { Success = false, ErrorMessage = $"Printer not ready: {status}" };
        }

        var tcs = new TaskCompletionSource<PrintResult>();
        var cultureRu = new CultureInfo("ru-RU");

        try
        {
            _printer!.SetGray(10);

            AddKeyValueText("Чек", salesReceipt.Number);
            AddLineWidthText();
            AddKeyValueText("Терминал", salesReceipt.TerminalNumber);

            if (salesReceipt.BaseType == BasePaymentType.NonCash && 
                salesReceipt.DerivedType == DerivedPaymentType.FuelCard)
            {
                AddKeyValueText("Карта", salesReceipt.CardNumber!);
                AddKeyValueText("Карта сокр", salesReceipt.CardNumber!);
            }

            AddLineWidthText("Продажа");
            AddKeyValueText(
                salesReceipt.ResourceName,
                $"= {salesReceipt.Amount.ToString(cultureRu)}");

            AddKeyValueText(
                salesReceipt.PricePerUnit.ToString(cultureRu),
                $"= {salesReceipt.SellingPrice.ToString(cultureRu)}");

            AddKeyValueText("Скидка", $"= {salesReceipt.Discount.ToString(cultureRu)}");
            AddKeyValueText("Итого", $"= {salesReceipt.TotalPrice.ToString(cultureRu)}");

            if (salesReceipt.BaseType == BasePaymentType.NonCash && 
                salesReceipt.DerivedType == DerivedPaymentType.FuelCard)
            {
                AddLineWidthText("Инфо по кошелькам");
            }

            AddLineWidthText();
            AddLeftText($"Оператор {salesReceipt.Operator}");
            AddLineWidthText();

            _printer.FeedLine(6);
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
        finally
        {
            Disconnect();
        }
        _logger.LogInformation($"Чек отпечатан");
        return await tcs.Task;
    }

    public async Task<PrintResult> PrintShiftReportAsync(ShiftReportDataDto reportData)
    {
        var receiptText = TextReportGenerator.FormatShiftReportText(reportData);
        var logger = App.Services!.GetRequiredService<ILogger<PageViewModelBase>>();
        
        var tcs = new TaskCompletionSource<PrintResult>();
        
        var shiftReportPage = new ShiftReportPageViewModel(
            logger, 
            receiptText, 
            async void () =>
            {
                try
                {
                    var result = await ExecutePrintShiftReportAsync(reportData);
                    tcs.TrySetResult(result);
                    _navigationService.GoBack();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                }
            },
            () => 
            {
                tcs.TrySetResult(new PrintResult { Success = false, ErrorMessage = "Печать отменена" });
                _navigationService.GoBack();
            }
        );
        
        _navigationService.NavigateToInstancePage(shiftReportPage);
        
        return await tcs.Task;
    }

    private async Task<PrintResult> ExecutePrintShiftReportAsync(ShiftReportDataDto reportData)
    {
        await ConnectAsync();
        CheckConnection();
        
        var status = await GetStatusAsync();
        if (status != PrinterStatus.Ready)
        {
            _logger.LogWarning($"Printer not ready: {status}");
            return new PrintResult { Success = false, ErrorMessage = $"Printer not ready: {status}" };
        }
        
        var tcs = new TaskCompletionSource<PrintResult>();
        var cultureRu = new CultureInfo("ru-RU");

        try
        {
            _printer!.SetGray(10);
            
            AddLineWidthText();
            AddKeyValueText("Чек", reportData.ReceiptNumber.ToString(cultureRu));
            AddLineWidthText();
            
            var title = reportData.ReportType switch
            {
                ShiftReportType.Interim => "Пром. отчёт",
                ShiftReportType.Final => "Итоговый отчёт",
                _ => ""
            };
            AddLineWidthText(title);
            
            AddKeyValueText("Эмитент", reportData.IssuerNumber);
            AddKeyValueText("Терминал", "#" + reportData.TerminalNumber);
            AddKeyValueText("Номер смены:", reportData.Shift.ShiftShopKey.ToString(cultureRu));
            AddKeyValueText("Начало:",
                reportData.Shift.ShiftDate != null ? reportData.Shift.ShiftDate!.Value.ToString(cultureRu) : "");
            AddKeyValueText("Конец:", DateTime.Now.ToString(cultureRu));
            AddLineWidthText();
            
            var issuersCount = reportData.SalesList
                .Select(x => x.ICI)
                .GroupBy(x => x!.Value)
                .Count();
            
            PrintOperationsOnIssuer(
                $"Эмитент {reportData.IssuerNumber}",
                reportData.SalesList.Where(x => x.ICI!.Value == Convert.ToInt32(reportData.IssuerNumber)),
                issuersCount > 1,
                cultureRu);

            if (issuersCount > 1)
                PrintOperationsOnIssuer(
                    $"Другие эмитенты",
                    reportData.SalesList.Where(x => x.ICI!.Value != Convert.ToInt32(reportData.IssuerNumber)),
                    true,
                    cultureRu);
            
            var totalData = new
            {
                TotalBaseCost = reportData.SalesList.Sum(x => x.SBC ?? 0),
                TotalSC = reportData.SalesList.Sum(x => x.SC ?? 0),
                TotalSBCR = reportData.SalesList.Sum(x => x.SBCR ?? 0),
                TotalSCR = reportData.SalesList.Sum(x => x.SCR ?? 0)
            };
            
            AddLineWidthText("Итого в чеке");
            AddCenteredText("Итого продаж");
            AddKeyValueText("Сумма баз.", totalData.TotalBaseCost.ToString(cultureRu));
            AddKeyValueText("Сумма скид.", totalData.TotalSC.ToString(cultureRu));

            AddCenteredText("Итого возвратов");
            AddKeyValueText("Сумма баз.", totalData.TotalSBCR.ToString(cultureRu));
            AddKeyValueText("Сумма скид.", totalData.TotalSCR.ToString(cultureRu));

            AddCenteredText("Всего продаж");
            AddKeyValueText("Сумма баз.", (totalData.TotalBaseCost - totalData.TotalSBCR).ToString(cultureRu));
            AddKeyValueText("Сумма скид.", (totalData.TotalSC - totalData.TotalSCR).ToString(cultureRu));

            AddCenteredText();
            AddLeftText($"Оператор: {reportData.OperatorName}");
            AddLineWidthText();
            _printer.FeedLine(2);
            AddLineWidthText();
            AddCenteredText("Подпись");
            
            _printer.FeedLine(6);
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
        finally
        {
            Disconnect();
        }
        _logger.LogInformation($"Чек отпечатан");
        return await tcs.Task;
    }

    private void PrintOperationsOnIssuer(
        string issuerName,
        IEnumerable<SalesReportResult> operations,
        bool isPrintTotal,
        CultureInfo culture)
    {
        AddCenteredText(issuerName);

        var salesReportResults = operations as SalesReportResult[] ?? operations.ToArray();
        
        foreach (var saleData in salesReportResults)
        {
            var resourceName = !string.IsNullOrEmpty(saleData.N) ? saleData.N! : "undefined";
            AddLineWidthText(resourceName);

            AddCenteredText("Продажи");
            AddKeyValueText("Ко-во", saleData.A != null ? saleData.A.Value.ToString(culture) : "0");
            AddKeyValueText("Сумма баз.", saleData.SBC != null ? saleData.SBC.Value.ToString(culture) : "0");
            AddKeyValueText("Сумма скид.", saleData.SC != null ? saleData.SC.Value.ToString(culture) : "0");

            AddCenteredText("Возвраты");
            AddKeyValueText("Ко-во", saleData.AR != null ? saleData.AR.Value.ToString(culture) : "0");
            AddKeyValueText("Сумма баз.", saleData.SBCR != null ? saleData.SBCR.Value.ToString(culture) : "0");
            AddKeyValueText("Сумма скид.", saleData.SCR != null ? saleData.SCR.Value.ToString(culture) : "0");

            AddCenteredText($"Итого по {resourceName}");
            AddKeyValueText("Ко-во", ((saleData.A ?? 0) - (saleData.AR ?? 0)).ToString(culture));
            AddKeyValueText("Сумма баз.", ((saleData.SBC ?? 0) - (saleData.SBCR ?? 0)).ToString(culture));
            AddKeyValueText("Сумма скид.", ((saleData.SC ?? 0) - (saleData.SCR ?? 0)).ToString(culture));
        }

        AddLineWidthText();
        
        if (!isPrintTotal) return;
        
        var totalData = new
        {
            TotalSBC = salesReportResults.Sum(x => x.SBC ?? 0),
            TotalSC = salesReportResults.Sum(x => x.SC ?? 0),
            TotalSBCR = salesReportResults.Sum(x => x.SBCR ?? 0),
            TotalSCR = salesReportResults.Sum(x => x.SCR ?? 0)
        };

        AddCenteredText("Итого продаж");
        AddKeyValueText("Сумма баз.", totalData.TotalSBC.ToString(culture));
        AddKeyValueText("Сумма скид.", totalData.TotalSC.ToString(culture));

        AddCenteredText("Итого возвратов");
        AddKeyValueText("Сумма баз.", totalData.TotalSBCR.ToString(culture));
        AddKeyValueText("Сумма скид.", totalData.TotalSCR.ToString(culture));

        AddCenteredText("Всего продаж");
        AddKeyValueText("Сумма баз.", (totalData.TotalSBC - totalData.TotalSBCR).ToString(culture));
        AddKeyValueText("Сумма скид.", (totalData.TotalSC - totalData.TotalSCR).ToString(culture));

        AddLineWidthText();
    }
    
    private async Task ConnectAsync()
    {
        if (_isConnected) return;

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
            return;
        }

        var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(10000));
        if (completedTask != tcs.Task)
        {
            if (_serviceConnection != null)
                _context.UnbindService(_serviceConnection);
            throw new TimeoutException("Connection to Sunyard service timed out.");
        }

        await tcs.Task;
    }

    private void Disconnect()
    {
        if (!_isConnected || _serviceConnection == null) 
            return;
        
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

    public async Task<PrinterStatus> GetStatusAsync()
    {
        CheckConnection();
        return await Task.Run(() =>
        {
            int status = _printer!.Status;
            return MapStatus(status);
        });
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
    private void AddCenteredText(string text = "")
    {
        var bundle = new Bundle();
        bundle.PutInt("font", IPrintConstant.IFontSize.Normal);
        bundle.PutInt("align", IPrintConstant.IAlign.Center);
        _printer!.AddText(bundle, text);
    }

    /// <summary>
    /// Добавить линию по ширине.
    /// </summary>
    /// <param name="text">Текст в середине линии.</param>
    private void AddLineWidthText(string text = "")
    {
        var widthPage = string.IsNullOrWhiteSpace(text) ? 55 : 45;
        var spacer = new string('-', (widthPage - text.Length) / 2);
        var inputText = spacer + text + spacer;

        AddCenteredText(inputText);
    }

    /// <summary>
    /// Добавить строку с ключом слева и значением справа.
    /// </summary>
    /// <param name="key">Ключ.</param>
    /// <param name="value">Значение.</param>
    private void AddKeyValueText(string key, string value)
    {
        var chips = new List<PrinterChip>
        {
            new(key, 0.5f, IPrintConstant.IAlign.Left),
            new(value, 0.5f, IPrintConstant.IAlign.Right)
        };
        _printer!.AddTextChips(chips);
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