using System.Xml.Serialization;

namespace Terminal.Application.Dtos;

public class CardInfoRequestDto
{
    [XmlElement("Request")]
    public RequestDto Request { get; set; } = new();
    
    [XmlElement("CardInfoList")]
    public List<CardInfoDto> CardInfoList { get; set; } = [];
}