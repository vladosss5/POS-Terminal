using System.Xml.Serialization;
using Terminal.Core.Enums;

namespace Terminal.Application.Dtos;

public class RequestDto
{
    [XmlElement("Command")]
    public DiscounterCommand Command { get; set; }
    
    [XmlElement("IssuerID")]
    public int IssuerId { get; set; }
    
    [XmlElement("ShopID")]
    public int ShopId { get; set; }
}