using System.Xml.Serialization;

namespace Terminal.Core.Entities.DiscountingDtos;

public class OutSaleInfoListDto
{
    [XmlElement("SaleInfo")]
    public List<OutSaleInfoDto> SaleInfos { get; set; } = [];
}