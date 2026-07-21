using System.Xml.Serialization;

namespace Terminal.Core.Entities.DiscountingDtos;

[XmlRoot("Root")]
public class OutXmlDto
{
    [XmlElement("OUT")]
    public object? Out { get; set; }

    [XmlElement("Request")]
    public OutRequestDto Request { get; set; } = new();

    [XmlElement("CartInfo")]
    public OutCartInfoDto CartInfo { get; set; } = new();

    [XmlElement("CardInfoMessageList")]
    public object? CardInfoMessageList { get; set; }

    [XmlElement("SaleInfoList")]
    public OutSaleInfoListDto SaleInfoList { get; set; } = new();

    [XmlElement("Parameters")]
    public OutParametersDto Parameters { get; set; } = new();
}