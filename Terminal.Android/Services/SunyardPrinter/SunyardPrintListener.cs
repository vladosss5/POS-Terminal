using System.Threading.Tasks;
using Android.OS;
using Com.Sunyard.Api.Printer;
using Microsoft.Extensions.Logging;
using Terminal.Core.Enums;
using Terminal.Core.Models;

namespace Terminal.Android.Services.SunyardPrinter;

public class SunyardPrintListener : IOnPrintListener.Stub
{
    private readonly TaskCompletionSource<PrintResult> _tcs;
    private readonly ILogger<SunyardPrintService> _logger;

    public SunyardPrintListener(
        TaskCompletionSource<PrintResult> tcs, 
        ILogger<SunyardPrintService> logger)
    {
        _tcs = tcs;
        _logger = logger;
    }

    public override void OnFinish()
    {
        _logger?.LogInformation("Print finished successfully.");
        _tcs.TrySetResult(new PrintResult { Success = true });
    }

    public override void OnError(int error)
    {
        _logger?.LogError($"Print failed with error code: {error}");
        _tcs.TrySetResult(new PrintResult
        {
            Success = false,
            ErrorMessage = $"Print error code: {error}",
            Status = MapErrorToStatus(error)
        });
    }

    private PrinterStatus? MapErrorToStatus(int error)
    {
        return error switch
        {
            IPrintConstant.IErrorCode.ErrorPaperended => PrinterStatus.PaperEnded,
            IPrintConstant.IErrorCode.ErrorHarderr => PrinterStatus.HardwareError,
            IPrintConstant.IErrorCode.ErrorOverheat => PrinterStatus.Overheat,
            IPrintConstant.IErrorCode.ErrorBufoverflow => PrinterStatus.BufferOverflow,
            IPrintConstant.IErrorCode.ErrorLowvol => PrinterStatus.LowVoltage,
            IPrintConstant.IErrorCode.ErrorPaperjam => PrinterStatus.PaperJam,
            IPrintConstant.IErrorCode.ErrorBusy => PrinterStatus.Busy,
            _ => null
        };
    }
}