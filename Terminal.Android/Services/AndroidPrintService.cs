using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Models;
using Terminal.Services;

namespace Terminal.Android.Services;

/// <summary>
/// Реализация сервиса печати для Android платформы.
/// </summary>
public class AndroidPrintService : IReceiptPrintService
{
    /// <inheritdoc cref="DialogPrintService" />
    private readonly DialogPrintService _dialogPrintService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public AndroidPrintService(ILogger<DialogPrintService> logger)
    {
        _dialogPrintService = new DialogPrintService(logger);
    }

    /// <inheritdoc/>
    public async Task<PrintResult> PrintSalesReceiptAsync(SalesReceipt salesReceipt)
    {
        return await _dialogPrintService.PrintSalesReceiptAsync(salesReceipt);
    }

    /// <inheritdoc/>
    public async Task<PrintResult> PrintShiftReportAsync(ShiftReportDataDto reportData)
    {
        return await _dialogPrintService.PrintShiftReportAsync(reportData);
    }

    /// <inheritdoc/>
    public async Task<PrintResult> PrintPriceChangeAsync(PriceChangeData changeData)
    {
        return await _dialogPrintService.PrintPriceChangeAsync(changeData);
    }
}