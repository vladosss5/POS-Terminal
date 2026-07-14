namespace Terminal.Core.Entities.DbEntities.MainDb;

public class Repayment
{
    /// <summary>
    /// Первичный ключ возврата (по магазину)
    /// </summary>
    public int RepaymentShopKey { get; set; }

    /// <summary>
    /// Сумма возврата
    /// </summary>
    public double? RepaymentValue { get; set; }

    /// <summary>
    /// Дата возврата
    /// </summary>
    public DateTime? RepaymentDate { get; set; }

    /// <summary>
    /// Тип возврата
    /// </summary>
    public byte? RepaymentType { get; set; }

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
    public long? TerminalKey { get; set; }

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
    /// Тип карты
    /// </summary>
    public byte? CardType { get; set; }
}
