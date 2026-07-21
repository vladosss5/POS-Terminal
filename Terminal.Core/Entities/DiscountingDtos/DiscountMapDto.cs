using System.Xml.Serialization;

namespace Terminal.Core.Entities.DiscountingDtos;

[XmlRoot("Discount_Map")]
public class DiscountMapDto
{
    [XmlElement("BonusMapKey")]
    public int BonusMapKey { get; set; }

    [XmlElement("BonusKey")]
    public int BonusKey { get; set; }

    [XmlElement("AccumulatorDiscountPercent")]
    public double AccumulatorDiscountPercent { get; set; }

    [XmlElement("AccumulatorDiscountCost")]
    public double AccumulatorDiscountCost { get; set; }

    [XmlElement("CasteKey")]
    public int CasteKey { get; set; }

    [XmlElement("AccumulatorDiscountVolume")]
    public double AccumulatorDiscountVolume { get; set; }

    [XmlElement("D_L")]
    public double Dl { get; set; }

    [XmlElement("D_H")]
    public double Dh { get; set; }

    [XmlElement("UnitType")]
    public int UnitType { get; set; }

    [XmlElement("PeriodType")]
    public int PeriodType { get; set; }

    [XmlElement("DiscountString")]
    public string DiscountString { get; set; } = "";

    [XmlElement("Cost")]
    public double Cost { get; set; }

    [XmlElement("AddDaysOfValidity")]
    public int AddDaysOfValidity { get; set; }

    [XmlElement("MapRowName")]
    public string? MapRowName { get; set; }

    [XmlElement("DiscountCostPercent")]
    public double DiscountCostPercent { get; set; }

    [XmlElement("GiftList")]
    public string? GiftList { get; set; }

    [XmlElement("DrawList")]
    public string? DrawList { get; set; }
}