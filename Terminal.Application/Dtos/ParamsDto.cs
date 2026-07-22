using System.Xml.Serialization;

namespace Terminal.Application.Dtos;

public class ParamsDto
{
    [XmlElement("BonusProgram")]
    public int BonusProgram { get; set; }
    
    [XmlElement("AjustAmount")]
    public int AdjustAmount { get; set; }
    
    [XmlElement("AjustAmountOnline")]
    public int AdjustAmountOnline { get; set; }
    
    [XmlElement("UserTimeout")]
    public int UserTimeout { get; set; }
    
    [XmlElement("ReadCard")]
    public int ReadCard { get; set; }
    
    [XmlElement("Version")]
    public int Version { get; set; }
    
    [XmlElement("Gift")]
    public int Gift { get; set; }
    
    [XmlElement("PrintData")]
    public int PrintData { get; set; }
    
    [XmlElement("PrintCommentData")]
    public int PrintCommentData { get; set; }
    
    [XmlElement("CouponData")]
    public int CouponData { get; set; }
}