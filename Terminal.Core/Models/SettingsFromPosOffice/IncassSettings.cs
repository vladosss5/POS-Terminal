using System.Xml.Serialization;

namespace Terminal.Core.Models.SettingsFromPosOffice;

/// <summary>
/// Настройки инкассации.
/// </summary>
public class IncassSettings
{
    /// <summary>
    /// Автоматическая инкассация.
    /// </summary>
    [XmlElement("auto")]
    public bool Auto { get; set; }
    
    /// <summary>
    /// Ожидание инкассации.
    /// </summary>
    /// <remarks>Устарел.</remarks>
    [XmlElement("wait")]
    public int Wait { get; set; }
    
    /// <summary>
    /// Демонстрационный режим инкассации.
    /// </summary>
    [XmlElement("demon")]
    public bool Demon { get; set; }
    
    /// <summary>
    /// Таймаут инкассации.
    /// </summary>
    [XmlElement("timeout")]
    public int Timeout { get; set; }
    
    /// <summary>
    /// Расписание инкассации.
    /// </summary>
    /// <remarks>Устарел.</remarks>
    [XmlElement("timetable")]
    public string? Timetable { get; set; }
}