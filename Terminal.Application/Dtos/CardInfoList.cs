using System.Xml.Serialization;

namespace Terminal.Application.Dtos;

/// <summary>
/// Список информации по картам.
/// </summary>
[XmlRoot("CardInfoList")]
public class CardInfoList
{
    [XmlElement("CardInfo")]
    public List<CardInfoDto> CardInfos { get; set; } = [];
}