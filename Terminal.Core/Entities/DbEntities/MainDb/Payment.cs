namespace Terminal.Core.Entities.DbEntities.MainDb;

public class Payment
{
/// <summary>
    /// Первичный ключ платежа (по магазину)
    /// </summary>
    public int PaymentShopKey { get; set; }

    /// <summary>
    /// Сумма платежа
    /// </summary>
    public decimal? PaymentSum { get; set; }

    /// <summary>
    /// Объём платежа
    /// </summary>
    public decimal? PaymentVolume { get; set; }

    /// <summary>
    /// Дата платежа
    /// </summary>
    public DateTime? PaymentDate { get; set; }

    /// <summary>
    /// Ключ магазина
    /// </summary>
    public int? ShopKey { get; set; }

    /// <summary>
    /// Ключ смены
    /// </summary>
    public int? ShiftKey { get; set; }

    /// <summary>
    /// Ключ терминала смены
    /// </summary>
    public int? ShiftTerminalKey { get; set; }

    /// <summary>
    /// Ключ терминала
    /// </summary>
    public decimal? TerminalKey { get; set; }

    /// <summary>
    /// Электронный номер карты
    /// </summary>
    public long? ElectronicNumber { get; set; }

    /// <summary>
    /// Признак отправки
    /// </summary>
    public bool? IsSent { get; set; }

    /// <summary>
    /// Код ошибки
    /// </summary>
    public int? ErrorCode { get; set; }

    /// <summary>
    /// Ключ карты эмитента
    /// </summary>
    public int? IssuerCardKey { get; set; }

    /// <summary>
    /// Ключ организации
    /// </summary>
    public int? OrganisationKey { get; set; }

    /// <summary>
    /// Ключ владельца
    /// </summary>
    public int? OwnerKey { get; set; }

    /// <summary>
    /// Ключ эмитента
    /// </summary>
    public int? IssuerKey { get; set; }

    /// <summary>
    /// ID приложения
    /// </summary>
    public int? ApplicationId { get; set; }

    /// <summary>
    /// Графический номер
    /// </summary>
    public decimal? GraphicalNumber { get; set; }

    /// <summary>
    /// Ключ коллекции
    /// </summary>
    public int? CollectionKey { get; set; }

    /// <summary>
    /// Ключ ресурса
    /// </summary>
    public int? ResourceKey { get; set; }

    /// <summary>
    /// Значение приложения
    /// </summary>
    public decimal? AppValue { get; set; }

    /// <summary>
    /// Общий ID приложения
    /// </summary>
    public int? CommonApplicationId { get; set; }

    /// <summary>
    /// GUID платежа
    /// </summary>
    public string? Guid { get; set; }

    /// <summary>
    /// Ключ корзины
    /// </summary>
    public int? ShoppingCartKey { get; set; }

    /// <summary>
    /// Флаги платежа
    /// </summary>
    public int? Flags { get; set; }

    /// <summary>
    /// Номер NZ (специфическое поле)
    /// </summary>
    public string? Nz { get; set; }

    /// <summary>
    /// Статус приложения
    /// </summary>
    public int? AppStatus { get; set; }
}
