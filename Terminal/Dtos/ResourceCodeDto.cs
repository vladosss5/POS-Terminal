using System;
using System.Globalization;

namespace Terminal.Dtos;

public class ResourceCodeDto
{
    /// <summary>
    /// Первичный ключ кода ресурса (топлива)
    /// </summary>
    public int FuelCodeKey { get; set; }
    
    /// <summary>
    /// Ключ ресурса
    /// </summary>
    public int ResourceKey { get; set; }

    /// <summary>
    /// Наименование ресурса
    /// </summary>
    public string ResourceName { get; set; } = "";
    
    /// <summary>
    /// Цена ресурса
    /// </summary>
    public decimal ResourcePrice { get; set; }

    /// <summary>
    /// Автосвойство для показа стоимости с десятичной частью через точку.
    /// </summary>
    public string ResourcePriceFormatted => Math.Round(ResourcePrice, 3).ToString(CultureInfo.InvariantCulture);
}