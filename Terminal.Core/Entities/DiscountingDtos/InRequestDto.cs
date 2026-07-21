using System.Xml.Serialization;

namespace Terminal.Core.Entities.DiscountingDtos;

public class InRequestDto
{
    [XmlElement("Command")]
    public int Command { get; set; }

    [XmlElement("ShopID")]
    public int ShopId { get; set; }

    [XmlElement("IssuerID")]
    public int IssuerId { get; set; }
}