using System.Threading.Tasks;
using Android.OS;
using Com.Sunyard.Api.Printer;
using Terminal.Core.Enums;
using Terminal.Core.Models;

namespace Terminal.Android.Services.SunyardPrinter;

public class SunyardPrintListener : Java.Lang.Object, IOnPrintListener
{
    private readonly TaskCompletionSource<PrintResult> _tcs;

    public SunyardPrintListener(TaskCompletionSource<PrintResult> tcs)
    {
        _tcs = tcs;
    }

    public void OnFinish()
    {
        _tcs.TrySetResult(new PrintResult { Success = true });
    }

    public void OnError(int error)
    {
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

    public IBinder? AsBinder() => null;
}