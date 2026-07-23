using System.Xml.Serialization;

namespace Terminal.Application.Dtos.CardInfoRoot;

public class CardInfoBase
{
    [XmlElement("Request")]
    public RequestDto Request { get; set; } = new();
    
    [XmlElement("CardInfoList")] 
    public CardInfoList CardInfoList { get; set; } = new ();
    
    [XmlElement("Parameters")]
    public ParamsDto Parameters { get; set; } = new();

    [XmlElement("CartInfo")]
    public CartInfoDto CartInfo { get; set; } = new();
}