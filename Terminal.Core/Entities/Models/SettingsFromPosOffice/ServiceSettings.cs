using System.Xml.Serialization;

namespace Terminal.Core.Entities.Models.SettingsFromPosOffice;

/// <summary>
/// Сервисные настройки. 
/// </summary>
public class ServiceSettings
{
    /// <summary>
    /// Пароль сервиса.
    /// </summary>
    [XmlElement("password")]
    public string Password { get; set; } = null!;
    
    /// <summary>
    /// Режим загрузки.
    /// </summary>
    [XmlElement("loadMode")]
    public int AuthorizeType { get; set; }
    
    /// <summary>
    /// Синхронизация времени.
    /// </summary>
    [XmlElement("synchroTime")]
    public bool SynchroTime { get; set; }
    
    /// <summary>
    /// Часовой пояс.
    /// </summary>
    [XmlElement("timezone")]
    public int Timezone { get; set; }
    
    /// <summary>
    /// Режим отладки.
    /// </summary>
    [XmlElement("debug")]
    public bool Debug { get; set; }
    
    /// <summary>
    /// Простой режим.
    /// </summary>
    [XmlElement("simpleMode")]
    public bool SimpleMode { get; set; }
    
    /// <summary>
    /// Использовать пин-пад.
    /// </summary>
    [XmlElement("usePinpad")]
    public bool UsePinpad { get; set; }
    
    /// <summary>
    /// Язык.
    /// </summary>
    [XmlElement("language")]
    public int Language { get; set; }
}