using Terminal.Core.Enums;

namespace Terminal.Core.Models;

/// <summary>
/// Модель результата печати.
/// </summary>
public class PrintResult
{
    /// <summary>
    /// Успешно?
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Сообщение об ошибке.
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// Статус принтера.
    /// </summary>
    public PrinterStatus? Status { get; set; }
}