using System.Xml.Serialization;

namespace Terminal.Application.Dtos.CardInfoRoot;

[XmlRoot("Root")]
public class CardInfoDtoRequestDto : CardInfoDtoBase
{
    [XmlElement("IN")] 
    public string Input { get; set; } = "";
}