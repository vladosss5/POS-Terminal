using System.Xml.Serialization;

namespace Terminal.Application.Dtos.CardInfoRoot;

[XmlRoot("Root")]
public class CardInfoResponseDto : CardInfoBase
{
    [XmlElement("OUT", IsNullable = true)]
    public string? Out { get; set; }
}