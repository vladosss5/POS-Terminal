namespace Terminal.Core.Models;

/// <summary>
/// Модель данных изменения цены ресурса.
/// </summary>
public class PriceChangeData
{
    /// <summary>
    /// Номер эмитента.
    /// </summary>
    public string IssuerNumber { get; set; } = null!;
    
    /// <summary>
    /// Номер терминала.
    /// </summary>
    public string TerminalNumber { get; set; } = null!;
    
    /// <summary>
    /// Дата и время изменения.
    /// </summary>
    public DateTime ChangingDateTime { get; set; }

    /// <summary>
    /// Название ресурса.
    /// </summary>
    public string ResourceName { get; set; } = null!;
    
    /// <summary>
    /// Цена до.
    /// </summary>
    public decimal PriceUpTo { get; set; }

    /// <summary>
    /// Цена после.
    /// </summary>
    public decimal PriceAfter { get; set; }
    
    /// <summary>
    /// Имя оператора.
    /// </summary>
    public string OperatorName { get; set; } = null!;
}