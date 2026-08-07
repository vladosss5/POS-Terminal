using System.Xml.Serialization;
using Terminal.Application.Helpers;
using Terminal.Core.Enums;

namespace Terminal.Application.Dtos;

[XmlRoot("CardInfo")]
public class CardInfoDto
{
    [XmlElement("DiscountVolume")]
    public decimal DiscountVolume { get; set; }
    
    [XmlElement("BonusValue")]
    public decimal BonusValue { get; set; }
    
    [XmlElement("BonusDiscount")]
    public decimal BonusDiscount { get; set; }
    
    [XmlElement("BonusCurrent")]
    public decimal BonusCurrent { get; set; }
    
    [XmlElement("RawApplicationValue")]
    public decimal RawApplicationValue { get; set; }
    
    [XmlElement("ApplicationValue")]
    public decimal ApplicationValue { get; set; }
    
    [XmlElement("DayApplicationValue")]
    public decimal DayApplicationValue { get; set; }
    
    [XmlElement("ApplicationLimit")]
    public decimal ApplicationLimit { get; set; }
    
    [XmlElement("DayApplicationLimit")]
    public decimal DayApplicationLimit { get; set; }
    
    [XmlElement("CommonDayApplicationValue")]
    public decimal CommonDayApplicationValue { get; set; }
    
    [XmlElement("Flags")]
    public string? Flags { get; set; }
    
    [XmlElement("ExFlags")]
    public int ExFlags { get; set; }
    
    [XmlElement("RestrictionFlags")]
    public int RestrictionFlags { get; set; }
    
    [XmlElement("BonusMode")]
    public int BonusMode { get; set; }
    
    [XmlElement("Index")]
    public int Index { get; set; }
    
    [XmlElement("ResourceSet")]
    public int ResourceSet { get; set; }
    
    [XmlElement("CardApplicationSchemeType")]
    public CardApplicationSchemeType ApplicationSchemeType { get; set; }
    
    [XmlElement("IssuerNet")]
    public int IssuerNet { get; set; }
    
    [XmlElement("IssuerCode")]
    public int IssuerCode { get; set; }
    
    [XmlElement("DiscountType")]
    public int DiscountType { get; set; }
    
    [XmlElement("StatusType")]
    public int StatusType { get; set; }
    
    [XmlElement("OrganizationCode")]
    public int OrganizationCode { get; set; }
    
    [XmlElement("PersonCode")]
    public int PersonCode { get; set; }
    
    [XmlElement("TransactionCount")]
    public int TransactionCount { get; set; }
    
    [XmlElement("PartCode")]
    public int PartCode { get; set; }
    
    [XmlElement("ApplicationID")]
    public int ApplicationId { get; set; }
    
    [XmlElement("CommonApplicationID")]
    public int CommonApplicationID { get; set; }
    
    [XmlElement("CommonApplicationCode")]
    public int CommonApplicationCode { get; set; }
    
    [XmlElement("ResourceCode")]
    public int ResourceCode { get; set; }
    
    [XmlElement("CardCategoryCode")]
    public int CardCategoryCode { get; set; }
    
    [XmlElement("CardApplicationType")]
    public int CardApplicationType { get; set; }
    
    [XmlElement("Scope")]
    public int Scope { get; set; }
    
    [XmlElement("Price")]
    public int Price { get; set; }
    
    [XmlElement("CardResourceSet")]
    public int CardResourceSet { get; set; }
    
    [XmlElement("CardResourceCode")]
    public int CardResourceCode { get; set; }
    
    [XmlElement("LastChangeCode")]
    public int LastChangeCode { get; set; }
    
    [XmlElement("CardType")]
    public int CardType { get; set; }
    
    [XmlElement("CouponType")]
    public int CouponType { get; set; }
    
    [XmlElement("AcquirerCode")]
    public int AcquirerCode { get; set; }
    
    [XmlElement("ExternalIssuerNet")]
    public int ExternalIssuerNet { get; set; }
    
    [XmlElement("ExternalIssuerCode")]
    public int ExternalIssuerCode { get; set; }
    
    [XmlElement("ElectronicNumber")]
    public int ElectronicNumber { get; set; }
    
    [XmlElement("GraphicalNumber")]
    public string? GraphicalNumber { get; set; }
    
    [XmlElement("QrCode")]
    public string? QrCode { get; set; }
    
    [XmlIgnore]
    private DateTime ExpirationDateValue { get; set; }
    
    [XmlElement("ExpirationDate")]
    public string ExpirationDate
    {
        get => XmlHelper.DateTimeToXml(ExpirationDateValue);
        set => ExpirationDateValue = XmlHelper.DateTimeFromXml(value);
    }
}