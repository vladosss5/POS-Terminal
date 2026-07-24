using System.Xml.Serialization;

namespace Terminal.Application.Dtos.CardInfoRoot;

[XmlRoot("Root")]
public class CardInfoDtoResponseDto : CardInfoDtoBase
{
    [XmlElement("OUT", IsNullable = true)]
    public string? Out { get; set; }
}