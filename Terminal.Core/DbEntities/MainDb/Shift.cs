namespace Terminal.Core.DbEntities.MainDb;

public partial class Shift
{
    /// <summary>
    /// Уникальный ключ записи смены в рамках магазина (первичный ключ)
    /// </summary>
    public int ShiftShopKey { get; set; }

    /// <summary>
    /// Номер/ключ смены
    /// </summary>
    public int? ShiftKey { get; set; }

    /// <summary>
    /// Ключ терминала
    /// </summary>
    public long? TerminalKey { get; set; }

    /// <summary>
    /// Код ошибки
    /// </summary>
    public int? ErrorCode { get; set; }

    /// <summary>
    /// Ключ магазина
    /// </summary>
    public int? ShopKey { get; set; }

    /// <summary>
    /// Дата смены
    /// </summary>
    public DateTime? ShiftDate { get; set; }

    /// <summary>
    /// ID оператора
    /// </summary>
    public int? OperatorId { get; set; }

    /// <summary>
    /// Признак открытой смены
    /// </summary>
    public bool? IsOpened { get; set; }

    /// <summary>
    /// Количество отправленных HTTP-запросов
    /// </summary>
    public int? HttpSend { get; set; }

    /// <summary>
    /// Количество полученных HTTP-ответов
    /// </summary>
    public int? HttpRecv { get; set; }
}
