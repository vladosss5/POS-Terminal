using System.Xml.Serialization;

namespace Terminal.Core.Entities.DiscountingDtos;

public class InSaleInfoListDto
{
    [XmlElement("SaleInfo")]
    public List<InSaleInfoDto> SaleInfos { get; set; } = [];
}