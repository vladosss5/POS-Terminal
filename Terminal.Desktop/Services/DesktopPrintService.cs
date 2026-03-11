using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Models;
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
    public DesktopPrintService(ILogger<DialogPrintService> logger)
    {
        _dialogPrintService = new DialogPrintService(logger);
    }

    /// <inheritdoc/>
    public async Task<PrintResult> PrintSalesReceiptAsync(SalesReceipt salesReceipt)
    {
        return await _dialogPrintService.PrintSalesReceiptAsync(salesReceipt);
    }
}