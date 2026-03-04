using Terminal.Core.Enums;
using Terminal.Core.Models;

namespace Terminal.Application.Interfaces.Services;

/// <summary>
/// Сервис для работы с принтером чеков.
/// </summary>
public interface IPrintService
{
    /// <summary>
    /// Принтер подключен?
    /// </summary>
    public bool IsConnected { get; }

    /// <summary>
    /// Подключение принтера.
    /// </summary>
    /// <returns>Удалось ли.</returns>
    public Task<bool> ConnectAsync();
    
    /// <summary>
    /// Отключение принтера.
    /// </summary>
    public void Disconnect();
    
    /// <summary>
    /// Получить статус принтера.
    /// </summary>
    /// <returns>Статус из enum.</returns>
    public Task<PrinterStatus> GetStatusAsync();

    /// <summary>
    /// Распечатать чек о покупке.
    /// </summary>
    /// <param name="salesReceipt">Чек о покупке.</param>
    /// <returns>Результат печати.</returns>
    public Task<PrintResult> PrintSalesReceiptAsync(SalesReceipt salesReceipt);
}