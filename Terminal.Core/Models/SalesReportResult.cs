namespace Terminal.Core.Models;

public class SalesReportResult
{
    /// <summary>
    /// Объём продаж (Amount).
    /// </summary>
    public decimal? A { get; set; }
    
    /// <summary>
    /// Базовая стоимость продажи (ShopBaseCost).
    /// </summary>
    public decimal? SBC { get; set; }
    
    /// <summary>
    /// Стоимость продажи с учётом скидок (ShopCost).
    /// </summary>
    public decimal? SC { get; set; }
    
    /// <summary>
    /// Объём возвратов (отрицательный Amount).
    /// </summary>
    public decimal? AR { get; set; }
    
    /// <summary>
    /// Сумма возвратов по базовой стоимости (отрицательный ShopBaseCost).
    /// </summary>
    public decimal? SBCR { get; set; }
    
    /// <summary>
    /// Сумма возвратов с учётом скидок (отрицательный ShopCost).
    /// </summary>
    public decimal? SCR { get; set; }
    
    /// <summary>
    /// Наименование ресурса.
    /// </summary>
    public string? N { get; set; }
    
    /// <summary>
    /// Сумма продаж.
    /// </summary>
    public decimal? ARA { get; set; }
    
    public decimal? SBCRA { get; set; }
    
    /// <summary>
    /// Номер карт эмитента.
    /// </summary>
    public int? ICI { get; set; }
    
    /// <summary>
    /// Номер организации.
    /// </summary>
    public int? OK { get; set; }
    
    /// <summary>
    /// Тип оплаты.
    /// </summary>
    public int? PT { get; set; }
}