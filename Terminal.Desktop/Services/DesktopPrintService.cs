using System.Threading.Tasks;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Models;

namespace Terminal.Desktop.Services;

public class DesktopPrintService : IReceiptPrintService
{
    public async Task<PrintResult> PrintSalesReceiptAsync(SalesReceipt salesReceipt)
    {
        throw new System.NotImplementedException();
    }
}