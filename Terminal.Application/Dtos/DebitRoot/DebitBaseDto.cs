using System.Xml.Serialization;

namespace Terminal.Application.Dtos.DebitRoot;

public class DebitBaseDto
{
    [XmlElement("Request")]
    public RequestDto Request { get; set; } = new();

    [XmlElement("CartInfo")]
    public CartInfoDto CartInfoDto { get; set; } = new();
    
    [XmlElement("CardInfoMessageList")]
    public string? CardInfoMessageList { get; set; }
    
    [XmlElement("SaleInfoList")]
    public SaleInfoListDto SaleInfoList { get; set; } = new();
    
    [XmlElement("Parameters")]
    public ParamsDto Parameters { get; set; } = new();
    
    [XmlElement("CardInfoList")] 
    public CardInfoList CardInfoList { get; set; } = new ();
}