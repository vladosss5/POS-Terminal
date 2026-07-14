using Terminal.Core.Enums;

namespace Terminal.Core.Entities.Models;

/// <summary>
/// Модель чека о покупке.
/// </summary>
public class SalesReceipt
{
    /// <summary>
    /// Номер чека.
    /// </summary>
    public string Number { get; set; } = null!;

    /// <summary>
    /// Номер терминала.
    /// </summary>
    public string TerminalNumber { get; set; } = null!;

    /// <summary>
    /// Номер карты.
    /// </summary>
    public string? CardNumber { get; set; }
    
    /// <summary>
    /// Дата и время покупки.
    /// </summary>
    public DateTime TransactionDateTime { get; set; }
    
    /// <summary>
    /// Имя ресурса.
    /// </summary>
    public string ResourceName { get; set; } = null!;
    
    /// <summary>
    /// Кол-во единиц ресурса.
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Цена за единицу.
    /// </summary>
    public decimal PricePerUnit { get; set; }

    /// <summary>
    /// Цена за товар.
    /// </summary>
    public decimal SellingPrice { get; set; }
    
    /// <summary>
    /// Скидка.
    /// </summary>
    public decimal Discount { get; set; }
    
    /// <summary>
    /// Итоговая сумма чека.
    /// </summary>
    public decimal TotalPrice { get; set; }
    
    /// <summary>
    /// Оператор.
    /// </summary>
    public string? Operator { get; set; }

    /// <summary>
    /// Базовый тип оплаты.
    /// </summary>
    public BasePaymentType BaseType { get; set; }
    
    /// <summary>
    /// Дополнительный тип оплаты.
    /// </summary>
    public DerivedPaymentType? DerivedType { get; set; }
}