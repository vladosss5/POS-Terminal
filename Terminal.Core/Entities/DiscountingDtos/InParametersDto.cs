using System.Xml.Serialization;

namespace Terminal.Core.Entities.DiscountingDtos;

public class InParametersDto
{
    [XmlElement("Version")]
    public int Version { get; set; }

    [XmlElement("CurrencyType")]
    public string CurrencyType { get; set; } = string.Empty;

    [XmlElement("AllowCardCategories")]
    public string? AllowCardCategories { get; set; }

    [XmlElement("AllowSpecialIssuers")]
    public string? AllowSpecialIssuers { get; set; }

    [XmlElement("DisableCardScopeCheck")]
    public int DisableCardScopeCheck { get; set; }

    [XmlElement("DisableDiscountCardExpiredCheck")]
    public int DisableDiscountCardExpiredCheck { get; set; }
}