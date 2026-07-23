using System.Xml.Serialization;

namespace Terminal.Application.Dtos;

[XmlRoot("CardInfo")]
public class CardInfoDto
{
    [XmlElement("BonusMode")]
    public int BonusMode { get; set; }
    
    [XmlElement("CardApplicationSchemeType")]
    public int CardApplicationSchemeType { get; set; }
    
    [XmlElement("IssuerNet")]
    public int IssuerNet { get; set; }
    
    [XmlElement("IssuerCode")]
    public int IssuerCode { get; set; }
    
    [XmlElement("OrganizationCode")]
    public int OrganizationCode { get; set; }
    
    [XmlElement("PersonCode")]
    public int PersonCode { get; set; }
    
    [XmlElement("CardType")]
    public int CardType { get; set; }
    
    [XmlElement("ElectronicNumber")]
    public int ElectronicNumber { get; set; }
    
    [XmlElement("GraphicalNumber")]
    public int GraphicalNumber { get; set; }
}