namespace Terminal.Core.DbEntities;

/// <summary>
/// Модель товара (топлива).
/// </summary>
public partial class ResourceCode
{
    /// <summary>
    /// Код топлива (PK).
    /// </summary>
    public int FuelCodeKey { get; set; }
    
    /// <summary>
    /// Код коллекции.
    /// </summary>
    public int CollectionKey { get; set; }
    
    /// <summary>
    /// Код ресурса.
    /// </summary>
    public int ResourceKey { get; set; }
    
    /// <summary>
    /// Название ресурса.
    /// </summary>
    public string? ResourceName { get; set; }
    
    /// <summary>
    /// Стоимость ресурса.
    /// </summary>
    public decimal? ResourcePrice { get; set; }

    /// <summary>
    /// Продажи зимой.
    /// </summary>
    public byte IsShow { get; set; }
    
    /// <summary>
    /// Плотность.
    /// </summary>
    public decimal? Density { get; set; }
    
    /// <summary>
    /// Температура.
    /// </summary>
    public decimal? Temperature { get; set; }
}
