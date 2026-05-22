using System.Text.Json.Serialization;

namespace Terminal.Core.Models.Settings;

/// <summary>
/// Модель конфигурации терминала.
/// </summary>
public class SettingsModel
{
    /// <summary>
    /// Список типов оплаты.
    /// </summary>
    [JsonPropertyName("PaymentTypes")]
    public List<SettingPaymentType>? PaymentTypes { get; set; }
    
    /// <summary>
    /// Время ожидания до отмены аутентификации в секундах.
    /// </summary>
    [JsonPropertyName("SecondsAuthenticationCanceled")]
    public short SecondsAuthenticationCanceled { get; set; }
    
    /// <summary>
    /// Настройки подключения к TMS.
    /// </summary>
    [JsonPropertyName("TmsConfiguration")]
    public TmsConfiguration? TmsConfiguration { get; set; }
}