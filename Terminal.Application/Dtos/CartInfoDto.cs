using System.Xml.Serialization;
using Terminal.Application.Helpers;

namespace Terminal.Application.Dtos;

public class CartInfoDto
{
    [XmlElement("BonusLimitValue")]
    public decimal BonusLimitValue { get; set; }
    
    [XmlElement("MaxBonusIn")]
    public decimal MaxBonusIn { get; set; }
    
    [XmlElement("MaxBonusInCost")]
    public decimal MaxBonusInCost { get; set; }
    
    [XmlElement("MinBonusIn")]
    public decimal MinBonusIn { get; set; }
    
    [XmlElement("MinBonusInCost")]
    public decimal MinBonusInCost { get; set; }
    
    [XmlElement("MaxBonusOut")]
    public decimal MaxBonusOut { get; set; }
    
    [XmlElement("MaxBonusOutCost")]
    public decimal MaxBonusOutCost { get; set; }
    
    [XmlElement("MinBonusOut")]
    public decimal MinBonusOut { get; set; }
    
    [XmlElement("MinBonusOutCost")]
    public decimal MinBonusOutCost { get; set; }
    
    [XmlElement("Flags")]
    public int Flags { get; set; }
    
    [XmlElement("BonusAction")]
    public int BonusAction { get; set; }
    
    [XmlElement("BonusProgram")]
    public int BonusProgram { get; set; }
    
    [XmlElement("TerminalNumber")]
    public int TerminalNumber { get; set; }

    [XmlIgnore] 
    private DateTime DateTimeValue { get; set; }
    
    [XmlElement("DateTime")]
    public string DateTineXml
    {
        get => XmlHelper.DateTimeToXml(DateTimeValue);
        set => DateTimeValue = XmlHelper.DateTimeFromXml(value);
    }

    [XmlElement("GUID")]
    public string? GuidId { get; set; } = Guid.Empty.ToString();
    
    [XmlElement("TransactionGUID")]
    public string TransactionGuid { get; set; } = Guid.Empty.ToString();
    
    [XmlElement("ClientPhone")]
    public string? ClientPhone { get; set; }
    
    [XmlElement("ClientEmail")]
    public string? ClientEmail { get; set; }
    
    [XmlElement("TerminalRequestID")]
    public int TerminalRequestId { get; set; }
}