namespace Terminal.Core.Entities.DbEntities.MainDb;

public class Request
{
    /// <summary>
    /// Ключ вендора (первичный ключ)
    /// </summary>
    public int VendorKey { get; set; }

    /// <summary>
    /// Ключ ресурса
    /// </summary>
    public int? ResourceKey { get; set; }

    /// <summary>
    /// Начальный объём
    /// </summary>
    public decimal? InitialVolume { get; set; }

    /// <summary>
    /// Завершённый объём
    /// </summary>
    public decimal? CompleteVolume { get; set; }

    /// <summary>
    /// Стоимость по магазину
    /// </summary>
    public decimal? ShopCost { get; set; }

    /// <summary>
    /// Тип запроса
    /// </summary>
    public int? RequestType { get; set; }

    /// <summary>
    /// Статус запроса
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// Окончание заправки
    /// </summary>
    public int? EndOfFilling { get; set; }

    /// <summary>
    /// Флаги
    /// </summary>
    public int? Flags { get; set; }

    /// <summary>
    /// Базовый тип
    /// </summary>
    public int? BaseType { get; set; }

    /// <summary>
    /// Производный тип
    /// </summary>
    public int? DerivedType { get; set; }

    /// <summary>
    /// Последний объём
    /// </summary>
    public decimal? LastVolume { get; set; }

    /// <summary>
    /// Ключ корзины
    /// </summary>
    public int? ShoppingCartKey { get; set; }
}
