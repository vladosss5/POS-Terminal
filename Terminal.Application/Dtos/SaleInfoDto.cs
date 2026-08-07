using System.Xml.Serialization;
using Terminal.Application.Helpers;

namespace Terminal.Application.Dtos;

/// <summary>
/// Информация о продаже (элемент SaleInfo)
/// </summary>
[XmlRoot("SaleInfo")]
public class SaleInfoDto
{
    /// <summary>
    /// Цена ресурса
    /// </summary>
    [XmlElement("ResourcePrice")]
    public decimal ResourcePrice { get; set; }

    /// <summary>
    /// Сумма запроса
    /// </summary>
    [XmlElement("RequestSum")]
    public decimal RequestSum { get; set; }

    /// <summary>
    /// Количество запроса
    /// </summary>
    [XmlElement("RequestAmount")]
    public decimal RequestAmount { get; set; }

    /// <summary>
    /// Начальное количество
    /// </summary>
    [XmlElement("InitialAmount")]
    public decimal InitialAmount { get; set; }

    /// <summary>
    /// Начальная стоимость
    /// </summary>
    [XmlElement("InitialCost")]
    public decimal InitialCost { get; set; }

    /// <summary>
    /// Начальные бонусы начисленные
    /// </summary>
    [XmlElement("InitialBonusIn")]
    public decimal InitialBonusIn { get; set; }

    /// <summary>
    /// Ставка налога
    /// </summary>
    [XmlElement("TaxRate")]
    public decimal TaxRate { get; set; }

    /// <summary>
    /// Плотность
    /// </summary>
    [XmlElement("Density")]
    public float Density { get; set; }

    /// <summary>
    /// Флаги
    /// </summary>
    [XmlElement("Flags")]
    public int Flags { get; set; }

    /// <summary>
    /// Идентификатор
    /// </summary>
    [XmlElement("Id")]
    public int Id { get; set; }

    /// <summary>
    /// Идентификатор запроса
    /// </summary>
    [XmlElement("RequestId")]
    public int RequestId { get; set; }

    /// <summary>
    /// Набор ресурсов
    /// </summary>
    [XmlElement("ResourceSet")]
    public int ResourceSet { get; set; }

    /// <summary>
    /// Код ресурса
    /// </summary>
    [XmlElement("ResourceCode")]
    public int ResourceCode { get; set; }

    /// <summary>
    /// Код ресурса эквайрера
    /// </summary>
    [XmlElement("AquirerResourceCode")]
    public int AcquirerResourceCode { get; set; }

    /// <summary>
    /// Базовый тип оплаты
    /// </summary>
    [XmlElement("BasePaymentType")]
    public int BasePaymentType { get; set; }

    /// <summary>
    /// Производный тип оплаты
    /// </summary>
    [XmlElement("DerivedPaymentType")]
    public int DerivedPaymentType { get; set; }

    /// <summary>
    /// Количество знаков после запятой для объема
    /// </summary>
    [XmlElement("VolumeDigits")]
    public int VolumeDigits { get; set; }

    /// <summary>
    /// Индекс начальной информации о карте
    /// </summary>
    [XmlElement("InitialCardInfoIndex")]
    public int InitialCardInfoIndex { get; set; }

    /// <summary>
    /// Индекс начальной информации о карте-модификаторе
    /// </summary>
    [XmlElement("InitialModifierCardInfoIndex")]
    public int InitialModifierCardInfoIndex { get; set; }

    /// <summary>
    /// Индекс рассчитанной информации о карте
    /// </summary>
    [XmlElement("CalculatedCardInfoIndex")]
    public int CalculatedCardInfoIndex { get; set; }

    /// <summary>
    /// Индекс рассчитанной информации о карте-модификаторе
    /// </summary>
    [XmlElement("CalculatedModifierCardInfoIndex")]
    public int CalculatedModifierCardInfoIndex { get; set; }

    /// <summary>
    /// Код поставщика
    /// </summary>
    [XmlElement("VendorCode")]
    public int VendorCode { get; set; }

    /// <summary>
    /// Код налога
    /// </summary>
    [XmlElement("TaxCode")]
    public int TaxCode { get; set; }

    /// <summary>
    /// Цена продажи (в копейках/миллимах)
    /// </summary>
    [XmlElement("SalePrice")]
    public decimal SalePrice { get; set; }

    /// <summary>
    /// Рассчитанная цена продажи
    /// </summary>
    [XmlElement("CalculatedSalePrice")]
    public decimal CalculatedSalePrice { get; set; }
    
    [XmlIgnore]
    public DateTime DateTimeValue { get; set; }

    /// <summary>
    /// Дата и время
    /// </summary>
    [XmlElement("DateTime")]
    public string? DateTime
    {
        get => XmlHelper.DateTimeToXml(DateTimeValue);
        set => DateTimeValue = XmlHelper.DateTimeFromXml(value);
    }

    /// <summary>
    /// Название ресурса
    /// </summary>
    [XmlElement("ResourceName")]
    public string? ResourceName { get; set; }

    /// <summary>
    /// GUID транзакции
    /// </summary>
    [XmlElement("TransactionGUID")]
    public string? TransactionGuid { get; set; }
}