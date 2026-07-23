using System.Xml.Serialization;

namespace Terminal.Application.Dtos;

public class CartInfoDto
{
    [XmlElement("Flags")]
    public string Flags { get; set; } = null!;

    [XmlElement("GUID")]
    public string GuidId { get; set; } = Guid.Empty.ToString();
    
    [XmlElement("TransactionGUID")]
    public string TransactionGuid { get; set; } = Guid.Empty.ToString();
}