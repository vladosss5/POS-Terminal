using System.Xml.Serialization;

namespace Terminal.Core.Entities.DiscountingDtos;

[XmlRoot("DiscountingDS")]
public class DiscountingDsDto
{
    [XmlElement("Bonus_Accumulation_Scheme")]
    public List<BonusAccumulationSchemeDto> BonusAccumulationSchemes { get; set; } = [];

    [XmlElement("Bonus_Charge_Map")]
    public List<BonusChargeMapDto> BonusChargeMaps { get; set; } = [];

    [XmlElement("Discount_Map")]
    public List<DiscountMapDto> DiscountMaps { get; set; } = [];

    [XmlElement("Discrimination")]
    public List<DiscriminationDto> Discriminations { get; set; } = [];

    [XmlElement("DiscriminationValue")]
    public List<DiscriminationValueDto> DiscriminationValues { get; set; } = [];

    [XmlElement("Limitation")]
    public List<LimitationDto> Limitations { get; set; } = [];
}