using System;
using System.Globalization;

namespace Terminal.ViewModels.Items;

/// <summary>
/// ДТО для вывода в список чеков.
/// </summary>
public record struct ReceiptForListingDto
{
    /// <summary>
    /// Id продажи/чека
    /// </summary>
    public int CheckNumber { get; set; }
    
    /// <summary>
    /// Наименование ресурса.
    /// </summary>
    public string ResourceName { get; set; }
    
    /// <summary>
    /// Кол-во проданного ресурса.
    /// </summary>
    public decimal ResourceCount { get; set; }
    
    /// <summary>
    /// ResourceCount с точкой как разделителем.
    /// </summary>
    public string ResourceCountFormatted => ResourceCount.ToString(CultureInfo.InvariantCulture);
    
    /// <summary>
    /// Цена за ресурс * кол-во
    /// </summary>
    public decimal PricePerItem { get; set; }
    
    /// <summary>
    /// PricePerItem с точкой как разделителем.
    /// </summary>
    public string PricePerItemFormatted => PricePerItem.ToString(CultureInfo.InvariantCulture);
    
    /// <summary>
    /// Дата и время продажи
    /// </summary>
    public string? SaleDate { get; set; }
    
    /// <summary>
    /// Итоговая цена чека.
    /// </summary>
    public decimal FullReceiptPrice { get; set; }
    
    /// <summary>
    /// FullReceiptPrice с точкой как разделителем.
    /// </summary>
    public string FullReceiptPriceFormatted => FullReceiptPrice.ToString(CultureInfo.InvariantCulture);
}