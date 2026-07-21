using System.Xml.Serialization;

namespace Terminal.Core.Entities.DiscountingDtos;

public class InSaleInfoDto
{
    [XmlElement("Id")]
    public int Id { get; set; }

    [XmlElement("RequestId")]
    public int RequestId { get; set; }

    [XmlElement("DateTime")]
    public string DateTime { get; set; } = string.Empty;

    [XmlElement("ResourceSet")]
    public int ResourceSet { get; set; }

    [XmlElement("ResourceCode")]
    public int ResourceCode { get; set; }

    [XmlElement("AquirerResourceCode")]
    public int AcquirerResourceCode { get; set; }

    [XmlElement("ResourcePrice")]
    public double ResourcePrice { get; set; }

    [XmlElement("ResourceName")]
    public string ResourceName { get; set; } = string.Empty;

    [XmlElement("BasePaymentType")]
    public int BasePaymentType { get; set; }

    [XmlElement("DerivedPaymentType")]
    public int DerivedPaymentType { get; set; }

    [XmlElement("PaymentTypeCode")]
    public int PaymentTypeCode { get; set; }

    [XmlElement("ModifierPaymentTypeCode")]
    public int ModifierPaymentTypeCode { get; set; }

    [XmlElement("RequestSum")]
    public double RequestSum { get; set; }

    [XmlElement("RequestAmount")]
    public double RequestAmount { get; set; }

    [XmlElement("Flags")]
    public int Flags { get; set; }

    [XmlElement("VolumeDigits")]
    public int VolumeDigits { get; set; }

    [XmlElement("CompleteAmount")]
    public double CompleteAmount { get; set; }

    [XmlElement("InitialAmount")]
    public double InitialAmount { get; set; }

    [XmlElement("InitialDiscount")]
    public double InitialDiscount { get; set; }

    [XmlElement("InitialCost")]
    public double InitialCost { get; set; }

    [XmlElement("InitialBonusIn")]
    public double InitialBonusIn { get; set; }

    [XmlElement("InitialBonusInCost")]
    public double InitialBonusInCost { get; set; }

    [XmlElement("InitialBonusOut")]
    public double InitialBonusOut { get; set; }

    [XmlElement("InitialBonusOutCost")]
    public double InitialBonusOutCost { get; set; }

    [XmlElement("InitialCardInfoIndex")]
    public int InitialCardInfoIndex { get; set; }

    [XmlElement("InitialModifierCardInfoIndex")]
    public int InitialModifierCardInfoIndex { get; set; }

    [XmlElement("CalculatedAmount")]
    public double CalculatedAmount { get; set; }

    [XmlElement("CalculatedDiscount")]
    public double CalculatedDiscount { get; set; }

    [XmlElement("CalculatedBonusIn")]
    public double CalculatedBonusIn { get; set; }

    [XmlElement("CalculatedBonusInCost")]
    public double CalculatedBonusInCost { get; set; }

    [XmlElement("CalculatedBonusOut")]
    public double CalculatedBonusOut { get; set; }

    [XmlElement("CalculatedBonusOutCost")]
    public double CalculatedBonusOutCost { get; set; }

    [XmlElement("CalculatedCardInfoIndex")]
    public int CalculatedCardInfoIndex { get; set; }

    [XmlElement("CalculatedModifierCardInfoIndex")]
    public int CalculatedModifierCardInfoIndex { get; set; }

    [XmlElement("VendorCode")]
    public int VendorCode { get; set; }

    [XmlElement("MessageCode")]
    public int MessageCode { get; set; }

    [XmlElement("TransactionGUID")]
    public string TransactionGuid { get; set; } = string.Empty;

    [XmlElement("CompleteDiscount")]
    public double CompleteDiscount { get; set; }

    [XmlElement("CompleteCost")]
    public double CompleteCost { get; set; }

    [XmlElement("CompleteBonusIn")]
    public double CompleteBonusIn { get; set; }

    [XmlElement("CompleteBonusInCost")]
    public double CompleteBonusInCost { get; set; }

    [XmlElement("CompleteBonusOut")]
    public double CompleteBonusOut { get; set; }

    [XmlElement("CompleteBonusOutCost")]
    public double CompleteBonusOutCost { get; set; }

    [XmlElement("GUID")]
    public string Guid { get; set; } = string.Empty;

    [XmlElement("ResourceGroupCode")]
    public int ResourceGroupCode { get; set; }

    [XmlElement("TaxCode")]
    public int TaxCode { get; set; }

    [XmlElement("TaxRate")]
    public double TaxRate { get; set; }

    [XmlElement("CouponIndexList")]
    public string? CouponIndexList { get; set; }

    [XmlElement("CalculatedCouponIndexList")]
    public string? CalculatedCouponIndexList { get; set; }

    [XmlElement("SalePrice")]
    public int SalePrice { get; set; }

    [XmlElement("CalculatedSalePrice")]
    public int CalculatedSalePrice { get; set; }

    [XmlElement("Density")]
    public double Density { get; set; }

    [XmlElement("CalculateDiscountList")]
    public string? CalculateDiscountList { get; set; }

    [XmlElement("CalculateAccumulateList")]
    public string? CalculateAccumulateList { get; set; }

    [XmlElement("CalculateBonusList")]
    public string? CalculateBonusList { get; set; }
}