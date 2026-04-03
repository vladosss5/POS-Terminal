using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.DbEntities;
using Terminal.Core.Enums;
using Terminal.Core.Models;
using Terminal.Services;

namespace Terminal.Android.Services;

/// <summary>
/// Реализация сервиса печати для Android платформы.
/// </summary>
/// <remarks>
/// НЕ ИСПОЛЬЗОВАТЬ!
/// Сервис выступает заглушкой для работы приложения на Android платформах кроме устройств Sunyard.
/// </remarks>
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
    public async Task<PrintResult> PrintShiftReportAsync(Shift shift, ShiftReportType reportType)
    {
        return await _dialogPrintService.PrintShiftReportAsync(shift, reportType);
    }
}