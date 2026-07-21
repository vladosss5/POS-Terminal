using System.Xml.Serialization;

namespace Terminal.Core.Entities.DiscountingDtos;

public class InCardInfoListDto
{
    [XmlElement("CardInfo")]
    public List<InCardInfoDto> CardInfos { get; set; } = [];
}