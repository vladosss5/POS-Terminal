using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Enums;
using Terminal.Core.Models;

namespace Terminal.Android.Services;

public class AndroidPrintService : IPrintService
{
    private readonly ILogger<AndroidPrintService> _logger;

    public AndroidPrintService(ILogger<AndroidPrintService> logger)
    {
        _logger = logger;
    }

    public bool IsConnected => false;

    public async Task<bool> ConnectAsync()
    {
        LogWarning();
        return false;
    }

    public void Disconnect()
    {
        LogWarning();
    }

    public async Task<PrinterStatus> GetStatusAsync()
    {
        LogWarning();
        return PrinterStatus.Unknown;
    }

    public async Task<PrintResult> PrintSalesReceiptAsync(SalesReceipt salesReceipt)
    {
        LogWarning();

        return new PrintResult
        {
            Success = false,
            ErrorMessage = "Принтер не работает на данном устройстве.",
            Status = PrinterStatus.Unknown
        };
    }

    private void LogWarning()
    {
        _logger.LogWarning("Принтер не работает на данном устройстве.");
    }
}