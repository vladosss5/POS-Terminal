using System.Xml.Serialization;

namespace Terminal.Core.Models.SettingsFromPosOffice;

public class CardSettings
{
    /// <summary>
    /// Скидка по карте.
    /// </summary>
    [XmlElement("discount")]
    public bool Discount { get; set; }
    
    /// <summary>
    /// Топливо по карте.
    /// </summary>
    [XmlElement("fuel")]
    public bool Fuel { get; set; }
    
    /// <summary>
    /// Выписка по карте.
    /// </summary>
    [XmlElement("statement")]
    public bool Statement { get; set; }
    
    /// <summary>
    /// Наличные по карте.
    /// </summary>
    [XmlElement("cash")]
    public bool Cash { get; set; }
    
    /// <summary>
    /// Магнитная карта.
    /// </summary>
    [XmlElement("magnetic")]
    public bool Magnetic { get; set; }
    
    /// <summary>
    /// Купон по карте.
    /// </summary>
    [XmlElement("coupon")]
    public bool Coupon { get; set; }
    
    /// <summary>
    /// Полная карта.
    /// </summary>
    [XmlElement("full")]
    public bool Full { get; set; }
    
    /// <summary>
    /// Дебетовая карта.
    /// </summary>
    [XmlElement("debet")]
    public bool Debet { get; set; }
    
    /// <summary>
    /// Кредитная карта.
    /// </summary>
    [XmlElement("credit")]
    public bool Credit { get; set; }
    
    /// <summary>
    /// Консигнация по карте.
    /// </summary>
    [XmlElement("consignment")]
    public bool Consignment { get; set; }
    
    /// <summary>
    /// Ресурс по карте.
    /// </summary>
    [XmlElement("resource")]
    public bool Resource { get; set; }
}