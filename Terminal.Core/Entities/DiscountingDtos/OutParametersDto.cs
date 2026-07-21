using System.Xml.Serialization;

namespace Terminal.Core.Entities.DiscountingDtos;

public class OutParametersDto
{
    [XmlElement("DisableCardScopeCheck")]
    public int DisableCardScopeCheck { get; set; }

    [XmlElement("DisableDiscountCardExpiredCheck")]
    public int DisableDiscountCardExpiredCheck { get; set; }

    [XmlElement("BonusProgram")]
    public int BonusProgram { get; set; }

    [XmlElement("AjustAmount")]
    public int AjustAmount { get; set; }

    [XmlElement("AjustAmountOnline")]
    public int AjustAmountOnline { get; set; }

    [XmlElement("UserTimeout")]
    public int UserTimeout { get; set; }

    [XmlElement("ReadCard")]
    public int ReadCard { get; set; }

    [XmlElement("Version")]
    public int Version { get; set; }

    [XmlElement("Gift")]
    public int Gift { get; set; }

    [XmlElement("PrintData")]
    public int PrintData { get; set; }

    [XmlElement("PrintCommentData")]
    public int PrintCommentData { get; set; }

    [XmlElement("ExtCommand")]
    public int ExtCommand { get; set; }

    [XmlElement("CouponData")]
    public int CouponData { get; set; }

    [XmlElement("LimitationObjectType")]
    public int LimitationObjectType { get; set; }

    [XmlElement("LimitationObjectKey")]
    public int LimitationObjectKey { get; set; }

    [XmlElement("LimitationType")]
    public int LimitationType { get; set; }

    [XmlElement("Value")]
    public int Value { get; set; }

    [XmlElement("LoggerType")]
    public int LoggerType { get; set; }

    [XmlElement("ShoppingCart")]
    public int ShoppingCart { get; set; }

    [XmlElement("ExtendedInfo")]
    public int ExtendedInfo { get; set; }

    [XmlElement("WarehouseToWorkState")]
    public int WarehouseToWorkState { get; set; }

    [XmlElement("AllowSpecialIssuers")]
    public string? AllowSpecialIssuers { get; set; }

    [XmlElement("AllowCardCategories")]
    public string? AllowCardCategories { get; set; }

    [XmlElement("CardOperationResult")]
    public string? CardOperationResult { get; set; }

    [XmlElement("Confirmation")]
    public string? Confirmation { get; set; }

    [XmlElement("CurrencyType")]
    public string CurrencyType { get; set; } = string.Empty;

    [XmlElement("PINRequestResult")]
    public string? PinRequestResult { get; set; }

    [XmlElement("PIN")]
    public string? Pin { get; set; }

    [XmlElement("PriceList")]
    public string? PriceList { get; set; }

    [XmlElement("Registration")]
    public string? Registration { get; set; }

    [XmlElement("CouponList")]
    public string? CouponList { get; set; }

    [XmlElement("CouponBaseList")]
    public string? CouponBaseList { get; set; }
}