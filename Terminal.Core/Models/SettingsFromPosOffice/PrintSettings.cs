using System.Xml.Serialization;

namespace Terminal.Core.Models.SettingsFromPosOffice;

/// <summary>
/// Настройки печати.
/// </summary>
public class PrintSettings
{
    /// <summary>
    /// Не печатать.
    /// </summary>
    [XmlElement("doNotPrint")]
    public bool DoNotPrint { get; set; }
    
    /// <summary>
    /// Показывать цену.
    /// </summary>
    [XmlElement("showPrice")]
    public bool ShowPrice { get; set; }
    
    /// <summary>
    /// Включать ли блог суммы в отчёт.
    /// </summary>
    [XmlElement("sum")]
    public bool Sum { get; set; }
    
    /// <summary>
    /// Бонусная программа.
    /// </summary>
    [XmlElement("bonusProgram")]
    public bool BonusProgram { get; set; }
    
    /// <summary>
    /// Удаленное обновление.
    /// </summary>
    [XmlElement("remoteUpdate")]
    public bool RemoteUpdate { get; set; }
    
    /// <summary>
    /// Блокировка продажи.
    /// </summary>
    [XmlElement("saleBlock")]
    public bool SaleBlock { get; set; }
    
    /// <summary>
    /// Тип чека.
    /// </summary>
    [XmlElement("chequeType")]
    public int ChequeType { get; set; }
    
    /// <summary>
    /// Печать инкассации.
    /// </summary>
    [XmlElement("incassPrint")]
    public bool IncassPrint { get; set; }
    
    /// <summary>
    /// Копия ресурса.
    /// </summary>
    [XmlElement("resourceCopy")]
    public bool ResourceCopy { get; set; }
    
    /// <summary>
    /// Копия скидки.
    /// </summary>
    [XmlElement("discountCopy")]
    public bool DiscountCopy { get; set; }
    
    /// <summary>
    /// Копия платежа.
    /// </summary>
    [XmlElement("paymentCopy")]
    public bool PaymentCopy { get; set; }
    
    /// <summary>
    /// Копия выписки.
    /// </summary>
    [XmlElement("statementCopy")]
    public bool StatementCopy { get; set; }
    
    /// <summary>
    /// Копия купона.
    /// </summary>
    [XmlElement("couponCopy")]
    public bool CouponCopy { get; set; }
    
    /// <summary>
    /// Количество отчетов.
    /// </summary>
    [XmlElement("reportCount")]
    public int ReportCount { get; set; }
    
    /// <summary>
    /// Общая сумма.
    /// </summary>
    [XmlElement("totalAmount")]
    public bool TotalAmount { get; set; }
    
    /// <summary>
    /// Общая скидка.
    /// </summary>
    [XmlElement("totalDiscount")]
    public bool TotalDiscount { get; set; }
    
    /// <summary>
    /// Тип отчета.
    /// </summary>
    [XmlElement("reportType")]
    public string? ReportPaymentTypes { get; set; }
    
    /// <summary>
    /// Состав отчета.
    /// </summary>
    [XmlElement("reportCompos")]
    public int ReportCompos { get; set; }
    
    /// <summary>
    /// Элементы отчета.
    /// </summary>
    [XmlElement("reportItems")]
    public int ReportItems { get; set; }
    
    /// <summary>
    /// Типы оплат транзакций включаемые в отчёт.
    /// </summary>
    [XmlElement("paymentTypes")]
    public int PaymentType { get; set; }
    
    /// <summary>
    /// Делить ли итоговый отчёт по эмитентам.
    /// </summary>
    [XmlElement("reportDevide")]
    public bool ReportDivide { get; set; }
    
    /// <summary>
    /// Организация в отчете.
    /// </summary>
    [XmlElement("reportOrg")]
    public bool ReportOrg { get; set; }
    
    /// <summary>
    /// Итог в отчете.
    /// </summary>
    [XmlElement("reportTotal")]
    public bool ReportTotal { get; set; }
    
    /// <summary>
    /// Разделение топлива.
    /// </summary>
    [XmlElement("devideFuels")]
    public bool DivideFuels { get; set; }
}