using System.Xml.Serialization;

namespace Terminal.Core.Entities.DiscountingDtos;

public class OutCartInfoDto
{
    [XmlElement("BonusLimitValue")]
    public double BonusLimitValue { get; set; }

    [XmlElement("MaxBonusIn")]
    public double MaxBonusIn { get; set; }

    [XmlElement("MaxBonusInCost")]
    public double MaxBonusInCost { get; set; }

    [XmlElement("MinBonusIn")]
    public double MinBonusIn { get; set; }

    [XmlElement("MinBonusInCost")]
    public double MinBonusInCost { get; set; }

    [XmlElement("MaxBonusOut")]
    public double MaxBonusOut { get; set; }

    [XmlElement("MaxBonusOutCost")]
    public double MaxBonusOutCost { get; set; }

    [XmlElement("MinBonusOut")]
    public double MinBonusOut { get; set; }

    [XmlElement("MinBonusOutCost")]
    public double MinBonusOutCost { get; set; }

    [XmlElement("Flags")]
    public int Flags { get; set; }

    [XmlElement("BonusAction")]
    public int BonusAction { get; set; }

    [XmlElement("BonusProgram")]
    public int BonusProgram { get; set; }

    [XmlElement("TerminalNumber")]
    public int TerminalNumber { get; set; }

    [XmlElement("DateTime")]
    public string DateTime { get; set; } = string.Empty;

    [XmlElement("Guid")]
    public string Guid { get; set; } = string.Empty;

    [XmlElement("TransactionGUID")]
    public string TransactionGuid { get; set; } = string.Empty;

    [XmlElement("ClientPhone")]
    public string? ClientPhone { get; set; }

    [XmlElement("ClientEmail")]
    public string? ClientEmail { get; set; }

    [XmlElement("TerminalRequestID")]
    public int TerminalRequestId { get; set; }
}