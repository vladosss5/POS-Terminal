using System.Xml.Serialization;

namespace Terminal.Core.Entities.DiscountingDtos;

public class OutRequestDto
{
    [XmlElement("ResultCode")]
    public int ResultCode { get; set; }

    [XmlElement("ResultCodeExt")]
    public int ResultCodeExt { get; set; }

    [XmlElement("Command")]
    public int Command { get; set; }

    [XmlElement("IssuerID")]
    public int IssuerId { get; set; }

    [XmlElement("ShopID")]
    public int ShopId { get; set; }

    [XmlElement("ShiftID")]
    public int ShiftId { get; set; }

    [XmlElement("Flags")]
    public int Flags { get; set; }

    [XmlElement("RequestTimeout")]
    public int RequestTimeout { get; set; }

    [XmlElement("ShopFlags")]
    public int ShopFlags { get; set; }

    [XmlElement("ShopState")]
    public int ShopState { get; set; }

    [XmlElement("ResultMessage")]
    public string? ResultMessage { get; set; }

    [XmlElement("ResultMessageExt")]
    public string? ResultMessageExt { get; set; }

    [XmlElement("ServiceList")]
    public string? ServiceList { get; set; }

    [XmlElement("RequestTypeList")]
    public string? RequestTypeList { get; set; }

    [XmlElement("PumpState")]
    public string? PumpState { get; set; }
}