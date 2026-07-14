using Terminal.Core.Entities.Models;

namespace Terminal.Core.Interfaces;

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
    /// <param name="reportData"></param>
    /// <returns>Результат печати.</returns>
    public Task<PrintResult> PrintShiftReportAsync(ShiftReportDataDto reportData);

    /// <summary>
    /// Печать смены цены.
    /// </summary>
    /// <param name="changeData">Данные по смене печати.</param>
    /// <returns>Результат печати.</returns>
    public Task<PrintResult> PrintPriceChangeAsync(PriceChangeData changeData);
}