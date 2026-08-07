using System.Xml.Serialization;

namespace Terminal.Application.Dtos.DebitRoot;

/// <summary>
/// Модель запроса дебетования к библиотеке скидок.
/// </summary>
[XmlRoot("Root")]
public class DebitRequestDto : DebitBaseDto
{
    [XmlElement("CardInfoModifierList")]
    public string? CardInfoModifierList { get; set; }
    
    [XmlElement("CouponInfoList")]
    public string? CouponInfoList { get; set; }
}