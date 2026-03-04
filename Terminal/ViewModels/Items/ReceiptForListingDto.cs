using System;

namespace Terminal.ViewModels.Items;

/// <summary>
/// ДТО для вывода в список чеков.
/// </summary>
public record struct ReceiptForListingDto
{
    /// <summary>
    /// Id продажи/чека
    /// </summary>
    public int TransactionShopKey { get; set; }
    
    /// <summary>
    /// Наименование ресурса.
    /// </summary>
    public string ResourceName { get; set; }
    
    /// <summary>
    /// Кол-во проданного ресурса.
    /// </summary>
    public decimal ResourceCount { get; set; }
    
    /// <summary>
    /// Цена за ресурс * кол-во
    /// </summary>
    public decimal PricePerItem { get; set; }
    
    /// <summary>
    /// Дата и время продажи
    /// </summary>
    public DateTime SaleDate { get; set; }
    
    /// <summary>
    /// Итоговая цена чека.
    /// </summary>
    public decimal FullReceiptPrice { get; set; }
}