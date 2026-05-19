using System.Xml.Serialization;

namespace Terminal.Core.Models.SettingsFromPosOffice;

/// <summary>
/// Модель настроек конфигурируемые через PosOffice.
/// </summary>
[XmlRoot("Root")]
public class SettingsFromPosOffice
{
    /// <summary>
    /// Настройки организации в печати.
    /// </summary>
    [XmlElement("organisation")]
    public Organisation Organisation { get; set; }
    
    /// <summary>
    /// Основные настройки.
    /// </summary>
    [XmlElement("mainSettings")]
    public MainSettings MainSettings { get; set; }
    
    /// <summary>
    /// Сервисные настройки.
    /// </summary>
    [XmlElement("serviceSettings")]
    public ServiceSettings ServiceSettings { get; set; }
}