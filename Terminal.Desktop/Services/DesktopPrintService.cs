using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Models;
using Terminal.Services;

namespace Terminal.Desktop.Services;

public class DesktopPrintService : IReceiptPrintService
{
    private readonly DialogPrintService _dialogPrintService;

    public DesktopPrintService(ILogger<DialogPrintService> logger)
    {
        _dialogPrintService = new DialogPrintService(logger);
    }

    public async Task<PrintResult> PrintSalesReceiptAsync(SalesReceipt salesReceipt)
    {
        return await _dialogPrintService.PrintSalesReceiptAsync(salesReceipt);
    }
}