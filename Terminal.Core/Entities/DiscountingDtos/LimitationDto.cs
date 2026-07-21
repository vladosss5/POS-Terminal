using System.Xml.Serialization;

namespace Terminal.Core.Entities.DiscountingDtos;

[XmlRoot("Limitation")]
public class LimitationDto
{
    [XmlElement("LimitationKey")]
    public int LimitationKey { get; set; }

    [XmlElement("LimitationObjectType")]
    public int LimitationObjectType { get; set; }

    [XmlElement("LimitationObjectKey")]
    public int LimitationObjectKey { get; set; }

    [XmlElement("LimitationValue")]
    public string LimitationValue { get; set; } = string.Empty;

    [XmlElement("LimitationType")]
    public int LimitationType { get; set; }

    [XmlElement("ActionType")]
    public int ActionType { get; set; }

    [XmlElement("LimitationName")]
    public string? LimitationName { get; set; }

    [XmlElement("Code1")]
    public int Code1 { get; set; }

    [XmlElement("Code2")]
    public int Code2 { get; set; }

    [XmlElement("Code3")]
    public int Code3 { get; set; }

    [XmlElement("StartDatetime")]
    public DateTime StartDatetime { get; set; }

    [XmlElement("StopDatetime")]
    public DateTime StopDatetime { get; set; }
}