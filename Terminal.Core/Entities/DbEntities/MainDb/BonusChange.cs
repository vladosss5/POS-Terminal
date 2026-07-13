namespace Terminal.Core.Entities.DbEntities.MainDb;

public class BonusChange
{
    /// <summary>
    /// Первичный ключ изменения бонусов (по магазину)
    /// </summary>
    public int BonusChangeShopKey { get; set; }

    /// <summary>
    /// ID приложения
    /// </summary>
    public int? ApplicationId { get; set; }

    /// <summary>
    /// Графический номер карты
    /// </summary>
    public decimal? GraphicalNumber { get; set; }

    /// <summary>
    /// Электронный номер карты
    /// </summary>
    public decimal? ElectronicNumber { get; set; }

    /// <summary>
    /// Изменение бонусов
    /// </summary>
    public decimal? BonusChange1 { get; set; }

    /// <summary>
    /// Ключ корзины
    /// </summary>
    public int? ShoppingCartKey { get; set; }

    /// <summary>
    /// ID терминала эмитента
    /// </summary>
    public int? IssuerTerminalId { get; set; }

    /// <summary>
    /// ID карты эмитента
    /// </summary>
    public int? IssuerCardId { get; set; }

    /// <summary>
    /// Ключ организации
    /// </summary>
    public int? OrganisationKey { get; set; }

    /// <summary>
    /// Ключ персоны / владельца
    /// </summary>
    public int? PersonKey { get; set; }

    /// <summary>
    /// Номер чека
    /// </summary>
    public int? CheckNumber { get; set; }

    /// <summary>
    /// Дата и время транзакции
    /// </summary>
    public DateTime? TransactionDatetime { get; set; }

    /// <summary>
    /// GUID набора товаров
    /// </summary>
    public string? SetOfGoodsGuid { get; set; }

    /// <summary>
    /// GUID товара
    /// </summary>
    public string? CommodityGuid { get; set; }

    /// <summary>
    /// Ключ терминала
    /// </summary>
    public decimal? TerminalKey { get; set; }

    /// <summary>
    /// Ключ смены
    /// </summary>
    public int? ShiftKey { get; set; }

    /// <summary>
    /// Код ошибки
    /// </summary>
    public int? ErrorCode { get; set; }

    /// <summary>
    /// Ключ магазина
    /// </summary>
    public int? ShopKey { get; set; }
}
