using System.Xml.Serialization;

namespace Terminal.Core.Entities.DiscountingDtos;

public class InCardInfoDto
{
    [XmlElement("Index")]
    public int Index { get; set; }

    [XmlElement("Flags")]
    public int Flags { get; set; }

    [XmlElement("ExFlags")]
    public int ExFlags { get; set; }

    [XmlElement("ResourceSet")]
    public int ResourceSet { get; set; }

    [XmlElement("ResourceCode")]
    public int ResourceCode { get; set; }

    [XmlElement("CardResourceSet")]
    public int CardResourceSet { get; set; }

    [XmlElement("CardResourceCode")]
    public int CardResourceCode { get; set; }

    [XmlElement("CardApplicationType")]
    public int CardApplicationType { get; set; }

    [XmlElement("CardApplicationSchemeType")]
    public int CardApplicationSchemeType { get; set; }

    [XmlElement("IssuerNet")]
    public int IssuerNet { get; set; }

    [XmlElement("IssuerCode")]
    public int IssuerCode { get; set; }

    [XmlElement("ElectronicNumber")]
    public long ElectronicNumber { get; set; }

    [XmlElement("GraphicalNumber")]
    public long GraphicalNumber { get; set; }

    [XmlElement("LastChangeCode")]
    public int LastChangeCode { get; set; }

    [XmlElement("CardType")]
    public int CardType { get; set; }

    [XmlElement("DiscountType")]
    public int DiscountType { get; set; }

    [XmlElement("DiscountVolume")]
    public int DiscountVolume { get; set; }

    [XmlElement("BonusValue")]
    public int BonusValue { get; set; }

    [XmlElement("BonusDiscount")]
    public int BonusDiscount { get; set; }

    [XmlElement("BonusCurrent")]
    public int BonusCurrent { get; set; }

    [XmlElement("RestrictionFlags")]
    public int RestrictionFlags { get; set; }

    [XmlElement("ExpirationDate")]
    public string ExpirationDate { get; set; } = string.Empty;

    [XmlElement("RawApplicationValue")]
    public int RawApplicationValue { get; set; }

    [XmlElement("ApplicationValue")]
    public int ApplicationValue { get; set; }

    [XmlElement("DayApplicationValue")]
    public int DayApplicationValue { get; set; }

    [XmlElement("ApplicationLimit")]
    public int ApplicationLimit { get; set; }

    [XmlElement("DayApplicationLimit")]
    public int DayApplicationLimit { get; set; }

    [XmlElement("OrganizationCode")]
    public int OrganizationCode { get; set; }

    [XmlElement("ApplicationID")]
    public int ApplicationId { get; set; }

    [XmlElement("CommonApplicationID")]
    public int CommonApplicationId { get; set; }

    [XmlElement("CommonApplicationCode")]
    public int CommonApplicationCode { get; set; }

    [XmlElement("CardCategoryCode")]
    public int CardCategoryCode { get; set; }

    [XmlElement("Scope")]
    public int Scope { get; set; }

    [XmlElement("Price")]
    public int Price { get; set; }

    [XmlElement("StatusType")]
    public int StatusType { get; set; }

    [XmlElement("CommonDayApplicationValue")]
    public int CommonDayApplicationValue { get; set; }

    [XmlElement("BonusMode")]
    public int BonusMode { get; set; }

    [XmlElement("PersonCode")]
    public int PersonCode { get; set; }

    [XmlElement("TransactionCount")]
    public int TransactionCount { get; set; }

    [XmlElement("CouponType")]
    public int CouponType { get; set; }

    [XmlElement("PartCode")]
    public int PartCode { get; set; }

    [XmlElement("AcquirerCode")]
    public int AcquirerCode { get; set; }

    [XmlElement("QRCode")]
    public string? QrCode { get; set; }

    [XmlElement("Email")]
    public string? Email { get; set; }

    [XmlElement("Phone")]
    public string? Phone { get; set; }
}