using System.Threading.Tasks;
using Com.Sunyard.Api.Printer;
using Terminal.Core.Entities.Models;
using Terminal.Core.Enums;
using Terminal.Core.Interfaces;

namespace Terminal.Android.Services.Sunyard.SunyardPrinter;

/// <summary>
/// Слушатель событий печати для принтера Sunyard.
/// Реализует интерфейс IOnPrintListener и получает уведомления о завершении печати или ошибках.
/// Используется для преобразования асинхронных callback'ов от принтера в Task-based модель через TaskCompletionSource.
/// </summary>
/// <remarks>
/// Экземпляр этого класса должен храниться в поле сервиса печати на время выполнения операции,
/// чтобы предотвратить его сборку сборщиком мусора до получения callback'а.
/// </remarks>
public class SunyardPrintListener : IOnPrintListener.Stub
{
    private readonly TaskCompletionSource<PrintResult> _tcs;
    private readonly ILoggingService _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="tcs">TaskCompletionSource для уведомления о результате печати.</param>
    /// <param name="logger">Логгер для записи событий печати.</param>
    public SunyardPrintListener(
        TaskCompletionSource<PrintResult> tcs, 
        ILoggingService logger)
    {
        _tcs = tcs;
        _logger = logger;
    }

    /// <summary>
    /// Вызывается при успешном завершении печати.
    /// Устанавливает результат в TaskCompletionSource с флагом Success = true.
    /// </summary>
    public override void OnFinish()
    {
        _logger.LogInformation("Print finished successfully.");
        _tcs.TrySetResult(new PrintResult { Success = true });
    }

    /// <summary>
    /// Вызывается при возникновении ошибки во время печати.
    /// Преобразует код ошибки в соответствующий статус принтера и устанавливает результат в TaskCompletionSource.
    /// </summary>
    /// <param name="error">Код ошибки из SDK принтера.</param>
    public override void OnError(int error)
    {
        _logger.LogError($"Print failed with error code: {error}");
        _tcs.TrySetResult(new PrintResult
        {
            Success = false,
            ErrorMessage = $"Print error code: {error}",
            Status = MapErrorToStatus(error)
        });
    }

    /// <summary>
    /// Преобразует код ошибки из SDK в перечисление PrinterStatus.
    /// </summary>
    /// <param name="error">Код ошибки из SDK.</param>
    /// <returns>Соответствующий статус принтера или null, если код не распознан.</returns>
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