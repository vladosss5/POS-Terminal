using System.Text.Json.Serialization;

namespace Terminal.Core.Models;

/// <summary>
/// Модель конфигурации типа оплаты.
/// </summary>
public class SettingPaymentType
{
    /// <summary>
    /// Отображаемое наименование.
    /// </summary>
    [JsonPropertyName("DisplayedName")]
    public string DisplayedName { get; set; } = null!;

    /// <summary>
    /// Базовый тип.
    /// </summary>
    [JsonPropertyName("BasePaymentType")]
    public int BaseType { get; set; }
    
    /// <summary>
    /// Дополнительный тип.
    /// </summary>
    [JsonPropertyName("DerivedPaymentType")]
    public int DerivedType { get; set; }
    
    /// <summary>
    /// True - когда включен.
    /// </summary>
    [JsonPropertyName("IsEnabled")]
    public bool IsEnabled { get; set; }
}