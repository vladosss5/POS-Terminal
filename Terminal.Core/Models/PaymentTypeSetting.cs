using Terminal.Core.Enums;

namespace Terminal.Core.Models;

/// <summary>
/// Модель конфигурации типа оплаты.
/// </summary>
public record struct PaymentTypeSetting
{
    /// <summary>
    /// Отображаемое наименование.
    /// </summary>
    public string DisplayedName { get; set; }
    
    /// <summary>
    /// Базовый тип.
    /// </summary>
    public BasePaymentType BaseType { get; set; }
    
    /// <summary>
    /// Дополнительный тип.
    /// </summary>
    public DerivedPaymentType DerivedType { get; set; }
    
    /// <summary>
    /// True - когда включен.
    /// </summary>
    public bool IsEnabled { get; set; }
}