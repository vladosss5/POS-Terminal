using System.Xml.Serialization;

namespace Terminal.Core.Models.SettingsFromPosOffice;

/// <summary>
/// Настройки организации для чеков.
/// </summary>
public class Organisation
{
    /// <summary>
    /// Сообщение вверху чека.
    /// </summary>
    [XmlElement("header")]
    public List<string>? Header { get; set; }
    
    /// <summary>
    /// Сообщение внизу чека.
    /// </summary>
    [XmlElement("footer")]
    public List<string>? Footer { get; set; }
}