using Terminal.Core.DbEntities;
using Terminal.Core.Enums;
using Terminal.Core.Models;

namespace Terminal.Application.Interfaces.Services;

/// <summary>
/// Сервис печати чеков.
/// </summary>
public interface IReceiptPrintService
{
    /// <summary>
    /// Напечатать чек о покупке.
    /// </summary>
    /// <param name="salesReceipt">Чек о покупке.</param>
    /// <returns>Результат печати.</returns>
    public Task<PrintResult> PrintSalesReceiptAsync(SalesReceipt salesReceipt);

    /// <summary>
    /// Напечатать отчёт за смену.
    /// </summary>
    /// <param name="shift">Смена.</param>
    /// <param name="reportType">Тип отчёта.</param>
    /// <returns>Результат печати.</returns>
    public Task<PrintResult> PrintShiftReportAsync(Shift shift, ShiftReportType reportType);
}