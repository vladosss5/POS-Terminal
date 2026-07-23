using System.Xml.Serialization;

namespace Terminal.Application.Dtos.CardInfoRoot;

[XmlRoot("Root")]
public class CardInfoRequestDto : CardInfoBase
{
    [XmlElement("IN")] 
    public string Input { get; set; } = "";
}