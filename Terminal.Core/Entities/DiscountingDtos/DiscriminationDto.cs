using System.Xml.Serialization;

namespace Terminal.Core.Entities.DiscountingDtos;

[XmlRoot("Discrimination")]
public class DiscriminationDto
{
    [XmlElement("BonusKey")]
    public int BonusKey { get; set; }

    [XmlElement("COD_L")]
    public int CodL { get; set; }

    [XmlElement("COD_O")]
    public int CodO { get; set; }

    [XmlElement("COD_R")]
    public int CodR { get; set; }

    [XmlElement("COD_A")]
    public int CodA { get; set; }

    [XmlElement("Name")]
    public string Name { get; set; } = string.Empty;

    [XmlElement("State")]
    public int State { get; set; }

    [XmlElement("StartDate")]
    public DateTime StartDate { get; set; }

    [XmlElement("FinishDate")]
    public DateTime FinishDate { get; set; }

    [XmlElement("OrganizationKey")]
    public int OrganizationKey { get; set; }

    [XmlElement("RequisitesSetKey")]
    public int RequisitesSetKey { get; set; }

    [XmlElement("ActionType")]
    public int ActionType { get; set; }

    [XmlElement("DiscountProgramType")]
    public int DiscountProgramType { get; set; }

    [XmlElement("DiscriminationTypeKey")]
    public int DiscriminationTypeKey { get; set; }

    [XmlElement("CasteKey")]
    public int CasteKey { get; set; }

    [XmlElement("Choice")]
    public int Choice { get; set; }

    [XmlElement("SyntheticAccountKey")]
    public int SyntheticAccountKey { get; set; }

    [XmlElement("BeneficiaryType")]
    public int BeneficiaryType { get; set; }

    [XmlElement("BonusProgramm")]
    public int BonusProgram { get; set; }

    [XmlElement("Priority")]
    public int Priority { get; set; }

    [XmlElement("GroupCode")]
    public int GroupCode { get; set; }

    [XmlElement("ShortName3")]
    public string? ShortName3 { get; set; }

    [XmlElement("COD_NB")]
    public int CodNb { get; set; }
}