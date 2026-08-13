using System.Threading.Tasks;
using Terminal.Core.Entities.Models;
using Terminal.Core.Interfaces;
using Terminal.Services;

namespace Terminal.Desktop.Services;

/// <summary>
/// Реализация сервиса вывода чеков для Desktop платформы.
/// </summary>
public class DesktopPrintService : IReceiptPrintService
{
    /// <inheritdoc cref="DialogPrintService" />
    private readonly DialogPrintService _dialogPrintService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public DesktopPrintService(ILoggingService logger)
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