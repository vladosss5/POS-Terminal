using System.Xml.Serialization;

namespace Terminal.Application.Dtos.DiscountRoot;

/// <summary>
/// Модель запроса предварительного прасчёта к библиотеке скидок.
/// </summary>
[XmlRoot("Root")]
public class DiscountRequestDto : DiscountBaseDto
{
    [XmlElement("CardInfoList")] 
    public CardInfoList CardInfoList { get; set; } = new ();
    
    [XmlElement("CardInfoModifierList")]
    public string? CardInfoModifierList { get; set; }
    
    [XmlElement("CouponInfoList")]
    public string? CouponInfoList { get; set; }
}