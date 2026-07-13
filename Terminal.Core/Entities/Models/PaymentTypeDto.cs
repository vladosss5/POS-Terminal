using Terminal.Core.Enums;

namespace Terminal.Core.Entities.Models;

/// <summary>
/// Модель конфигурации типа оплаты.
/// </summary>
public class PaymentTypeDto
{
    /// <summary>
    /// Отображаемое наименование.
    /// </summary>
    public string DisplayedName { get; set; } = null!;

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