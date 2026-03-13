using System.Text.Json.Serialization;

namespace Terminal.Core.Models;

/// <summary>
/// DTO для десериализации из JSON (точное соответствие формату)
/// </summary>
public class PaymentTypeSettingDto
{
    [JsonPropertyName("DisplayedName")]
    public string DisplayedName { get; set; } = string.Empty;
    
    [JsonPropertyName("BasePaymentType")]
    public int BasePaymentType { get; set; }
    
    [JsonPropertyName("DerivedPaymentType")]
    public int DerivedPaymentType { get; set; }
    
    [JsonPropertyName("IsEnabled")]
    public bool IsEnabled { get; set; }
}