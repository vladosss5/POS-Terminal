using System.Xml.Serialization;

namespace Terminal.Application.Dtos.CardInfoRoot;

public class CardInfoDtoBase
{
    [XmlElement("Request")]
    public RequestDto Request { get; set; } = new();
    
    [XmlElement("CartInfo")]
    public CartInfoDto CartInfo { get; set; } = new();
    
    [XmlElement("CardInfoMessageList")]
    public string? CardInfoMessageList { get; set; }
    
    [XmlElement("SaleInfoList")]
    public string? SaleInfoList { get; set; }
    
    [XmlElement("Parameters")]
    public ParamsDto Parameters { get; set; } = new();
    
    [XmlElement("CardInfoList")] 
    public CardInfoList CardInfoList { get; set; } = new ();
    
    [XmlElement("CardInfoModifierList")]
    public string? CardInfoModifierList { get; set; }
    
    [XmlElement("CouponInfoList")]
    public string? CouponInfoList { get; set; }
}