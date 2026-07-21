using System.Xml.Serialization;

namespace Terminal.Core.Entities.DiscountingDtos;

[XmlRoot("DiscriminationValue")]
public class DiscriminationValueDto
{
    [XmlElement("DiscriminationValueKey")]
    public int DiscriminationValueKey { get; set; }

    [XmlElement("DiscountValue")]
    public double DiscountValue { get; set; }

    [XmlElement("ResourceKey")]
    public int ResourceKey { get; set; }

    [XmlElement("ResourceType")]
    public int ResourceType { get; set; }

    [XmlElement("BonusKey")]
    public int BonusKey { get; set; }

    [XmlElement("DiscountString")]
    public string DiscountString { get; set; } = string.Empty;

    [XmlElement("AddDaysOfValidity")]
    public int AddDaysOfValidity { get; set; }

    [XmlElement("CardValue")]
    public int CardValue { get; set; }

    [XmlElement("ValueRowName")]
    public string? ValueRowName { get; set; }
}