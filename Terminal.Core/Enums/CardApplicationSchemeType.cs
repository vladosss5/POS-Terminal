using System.Xml.Serialization;

namespace Terminal.Core.Enums;

/// <summary>
/// Схемы применения карты.
/// </summary>
public enum CardApplicationSchemeType
{
    /// <summary>
    /// Топливная.
    /// </summary>
    [XmlEnum("0")]
    Resource,
    
    /// <summary>
    /// Скидочная.
    /// </summary>
    [XmlEnum("1")]
    Discount,
    
    /// <summary>
    /// Банковская.
    /// </summary>
    [XmlEnum("2")]
    Bank,
    
    /// <summary>
    /// Бонусная.
    /// </summary>
    [XmlEnum("3")]
    Bonus,
    
    /// <summary>
    /// Талоны.
    /// </summary>
    [XmlEnum("4")]
    Statement,
    
    [XmlEnum("5")]
    Coupon,
    
    [XmlEnum("6")]
    Ticket,
    
    [XmlEnum("7")]
    BankQr,
    
    /// <summary>
    /// Любая карта.
    /// </summary>
    [XmlEnum("8")]
    Max
}