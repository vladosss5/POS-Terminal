using System.Xml.Serialization;

namespace Terminal.Core.Entities.Models.SettingsFromPosOffice;

/// <summary>
/// Основные настройки.
/// </summary>
public class MainSettings
{
    /// <summary>
    /// Режим работы.
    /// </summary>
    [XmlElement("mode")]
    public ModeSettings Mode { get; set; }
    
    /// <summary>
    /// Настройки карт.
    /// </summary>
    [XmlElement("card")]
    public CardSettings Card { get; set; }
    
    /// <summary>
    /// Настройки печати.
    /// </summary>
    [XmlElement("print")]
    public PrintSettings Print { get; set; }
    
    /// <summary>
    /// Настрйоки инкассации.
    /// </summary>
    [XmlElement("incass")]
    public IncassSettings Incass { get; set; }
    
    /// <summary>
    /// Настройка округления чисел.
    /// </summary>
    [XmlElement("digits")]
    public DigitsSettings Digits { get; set; }
    
    /// <summary>
    /// Лимит объема.
    /// </summary>
    [XmlElement("limitVolume")]
    public int LimitVolume { get; set; }
    
    /// <summary>
    /// Ключ магазина.
    /// </summary>
    [XmlElement("shopKey")]
    public int ShopKey { get; set; }
    
    /// <summary>
    /// Конфигурация работы.
    /// </summary>
    [XmlElement("workConfig")]
    public int WorkConfig { get; set; }
}