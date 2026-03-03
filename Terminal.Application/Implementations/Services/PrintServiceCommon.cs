using Terminal.Application.Interfaces.Services;
using Terminal.Core.Enums;
using Terminal.Core.Models;

namespace Terminal.Application.Implementations.Services;

public class PrintServiceCommon : IPrintService
{
    public bool IsConnected { get; }
    public async Task<bool> ConnectAsync()
    {
        throw new NotImplementedException();
    }

    public void Disconnect()
    {
        throw new NotImplementedException();
    }

    public async Task<PrinterStatus> GetStatusAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<PrintResult> PrintSalesReceiptAsync(SalesReceipt salesReceipt)
    {
        throw new NotImplementedException();
    }
}