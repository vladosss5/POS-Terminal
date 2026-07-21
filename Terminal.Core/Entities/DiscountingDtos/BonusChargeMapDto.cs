using System.Xml.Serialization;

namespace Terminal.Core.Entities.DiscountingDtos;

[XmlRoot("Bonus_Charge_Map")]
public class BonusChargeMapDto
{
    [XmlElement("AccrualTarifKey")]
    public int AccrualTariffKey { get; set; }

    [XmlElement("AccumulationKey")]
    public int AccumulationKey { get; set; }

    [XmlElement("BonusVolume")]
    public double BonusVolume { get; set; }

    [XmlElement("BonusCost")]
    public double BonusCost { get; set; }

    [XmlElement("ResourceType")]
    public int ResourceType { get; set; }

    [XmlElement("D_L")]
    public double Dl { get; set; }

    [XmlElement("D_H")]
    public double Dh { get; set; }

    [XmlElement("DiscountString")]
    public string DiscountString { get; set; } = string.Empty;

    [XmlElement("UnitType")]
    public int UnitType { get; set; }

    [XmlElement("DiscountCost")]
    public double DiscountCost { get; set; }

    [XmlElement("DiscountVolume")]
    public double DiscountVolume { get; set; }

    [XmlElement("MapRowName")]
    public string? MapRowName { get; set; }

    [XmlElement("DrawList")]
    public string? DrawList { get; set; }

    [XmlElement("GoodsListType")]
    public int GoodsListType { get; set; }
}