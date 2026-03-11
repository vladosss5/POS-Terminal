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
}