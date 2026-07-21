using System.Xml.Serialization;

namespace Terminal.Core.Entities.DiscountingDtos;

[XmlRoot("Root")]
public class InXmlDto
{
    [XmlElement("Request")]
    public InRequestDto Request { get; set; } = new();

    [XmlElement("CartInfo")]
    public InCartInfoDto CartInfo { get; set; } = new();

    [XmlElement("CardInfoList")]
    public InCardInfoListDto CardInfoList { get; set; } = new();

    [XmlElement("CardInfoModifierList")]
    public object? CardInfoModifierList { get; set; }

    [XmlElement("CouponInfoList")]
    public object? CouponInfoList { get; set; }

    [XmlElement("SaleInfoList")]
    public InSaleInfoListDto SaleInfoList { get; set; } = new();

    [XmlElement("Parameters")]
    public InParametersDto Parameters { get; set; } = new();
}