using System.Xml.Serialization;

namespace Terminal.Application.Dtos;

/// <summary>
/// Список информации по продаже.
/// </summary>
[XmlRoot("SaleInfoList")]
public class SaleInfoListDto
{
    [XmlElement("SaleInfo")]
    public List<SaleInfoDto> SaleInfos { get; set; } = [];
}