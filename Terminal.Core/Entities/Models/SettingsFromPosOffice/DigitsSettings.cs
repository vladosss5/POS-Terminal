using System.Xml.Serialization;

namespace Terminal.Core.Entities.Models.SettingsFromPosOffice;

/// <summary>
/// Настройка округления чисел.
/// </summary>
public class DigitsSettings
{
    /// <summary>
    /// Количество знаков объема.
    /// </summary>
    [XmlElement("volumeCount")]
    public int VolumeCount { get; set; }
    
    /// <summary>
    /// Количество знаков суммы.
    /// </summary>
    [XmlElement("amountCount")]
    public int AmountCount { get; set; }
    
    /// <summary>
    /// Количество знаков обременения.
    /// </summary>
    [XmlElement("bonusCount")]
    public int BonusCount { get; set; }
}