using System.ComponentModel.DataAnnotations.Schema;
using Terminal.Core.Enums;

namespace Terminal.Core.DbEntities;

/// <summary>
/// Модель продажи.
/// </summary>
public class Selling
{
    /// <summary>
    /// Первичный ключ продажи (по магазину)
    /// </summary>
    public int TransactionShopKey { get; set; }

    /// <summary>
    /// Номер смены (FK)
    /// </summary>
    public int? ShiftKey { get; set; }

    /// <summary>
    /// Номер магазина
    /// </summary>
    public int? ShopKey { get; set; }

    /// <summary>
    /// Ключ терминала (задаётся единоразово)
    /// </summary>
    public long? TerminalKey { get; set; }

    /// <summary>
    /// Дата и время транзакции
    /// </summary>
    public DateTime? TransactionDatetime { get; set; }

    /// <summary>
    /// Графический номер
    /// </summary>
    public double? GraphicalNumber { get; set; }

    /// <summary>
    /// Электронный номер карты
    /// </summary>
    public long? ElectronicNumber { get; set; }

    /// <summary>
    /// Базовый тип оплаты.
    /// </summary>
    public BasePaymentType? BaseType { get; set; }

    /// <summary>
    /// Производный тип оплаты.
    /// </summary>
    public DerivedPaymentType? DerivedType { get; set; }

    /// <summary>
    /// Кол-во проданного товара.
    /// </summary>
    public decimal? Amount { get; set; }

    /// <summary>
    /// Итоговая стоимость покупки (с учётом скидок) за транзакцию.
    /// </summary>
    public decimal? ShopCost { get; set; }

    /// <summary>
    /// Базовая стоимость покупки (без скидок) за транзакцию.
    /// </summary>
    public decimal? ShopBaseCost { get; set; } 
    
    /// <summary>
    /// Цена товара за ед. с учётом скидок.
    /// </summary>
    /// <remarks>
    /// Рассчитывается как ShopCost/Amount
    /// </remarks>
    public decimal? SellingPrice { get; set; }

    /// <summary>
    /// Ключ корзины.
    /// </summary>
    public int? ShoppingCartKey { get; set; }

    /// <summary>
    /// Ключ ресурса (FK).
    /// </summary>
    public int? ResourceKey { get; set; }

    /// <summary>
    /// Ключ коллекции.
    /// </summary>
    public int? CollectionKey { get; set; }

    /// <summary>
    /// Код ресурса (ссылка на ResourceCode.FuelCodeKey).
    /// </summary>
    public int? ResourceCode { get; set; }

    /// <summary>
    /// Ключ ресурса по магазину.
    /// </summary>
    public int? ResourceShopKey { get; set; }

    /// <summary>
    /// Наименование ресурса.
    /// </summary>
    public string? ResourceName { get; set; }

    /// <summary>
    /// ID терминала эмитента.
    /// </summary>
    public int? IssuerTerminalId { get; set; }

    /// <summary>
    /// ID карты эмитента.
    /// </summary>
    public int? IssuerCardId { get; set; }

    /// <summary>
    /// Ключ организации.
    /// </summary>
    public int? OrganisationKey { get; set; }

    /// <summary>
    /// Ключ персоны.
    /// </summary>
    public int? PersonKey { get; set; }
    
    /// <summary>
    /// Имя оператора.
    /// </summary>
    [NotMapped] public string? PersonName { get; set; }

    /// <summary>
    /// ID приложения.
    /// </summary>
    public int? ApplicationId { get; set; }

    /// <summary>
    /// Количество транзакций (всегда 1 кроме талонов и ТК).
    /// </summary>
    public int? TransactionCount { get; set; }

    /// <summary>
    /// Подпись не используется.
    /// </summary>
    public string? Sign { get; set; }

    /// <summary>
    /// Статус приложения.
    /// </summary>
    public int? AppStatus { get; set; }

    /// <summary>
    /// Режим приложения.
    /// </summary>
    public int? AppMode { get; set; }

    /// <summary>
    /// Лимит приложения.
    /// </summary>
    public float? AppLimit { get; set; }

    /// <summary>
    /// Значение приложения.
    /// </summary>
    public float? AppValue { get; set; }

    /// <summary>
    /// Второй лимит приложения.
    /// </summary>
    public float? AppSecondLimit { get; set; }

    /// <summary>
    /// Второе значение приложения.
    /// </summary>
    public double? AppSecondValue { get; set; }

    /// <summary>
    /// Номер чека.
    /// </summary>
    public int? CheckNumber { get; set; }

    /// <summary>
    /// Период валидности.
    /// </summary>
    public long? ValidityPeriod { get; set; }

    /// <summary>
    /// Общий ID приложения.
    /// </summary>
    public int? CommonApplicationId { get; set; }

    /// <summary>
    /// GUID транзакции.
    /// </summary>
    public Guid? Guid { get; set; }

    /// <summary>
    /// Код ошибки.
    /// </summary>
    public int? ErrorCode { get; set; }

    /// <summary>
    /// Бонусы начисленные.
    /// </summary>
    public decimal? BonusIn { get; set; }

    /// <summary>
    /// Бонусы списанные.
    /// </summary>
    public decimal? BonusOut { get; set; }

    /// <summary>
    /// Стоимость начисленных бонусов.
    /// </summary>
    public decimal? BonusInCost { get; set; }

    /// <summary>
    /// Стоимость списанных бонусов.
    /// </summary>
    public decimal? BonusOutCost { get; set; }

    /// <summary>
    /// Признак отчёта по счёту.
    /// </summary>
    public int? IsAccountRep { get; set; }

    /// <summary>
    /// Запрошенный объём.
    /// </summary>
    public decimal? RequestedAmount { get; set; }

    /// <summary>
    /// Запрошенная стоимость.
    /// </summary>
    public decimal? RequestedCost { get; set; }

    /// <summary>
    /// Цена за ед. без скидок.
    /// </summary>
    /// <remarks>
    /// Рассчитывается как ShopBaseCost/Amount
    /// </remarks>
    public decimal? BasePrice { get; set; }

    /// <summary>
    /// Стоимость для клиента.
    /// </summary>
    /// <remarks>
    /// То же что и RequestedCost.
    /// </remarks>
    public decimal? ClientCost { get; set; }

    /// <summary>
    /// Флаги запроса.
    /// </summary>
    public int? RequestFlags { get; set; }

    /// <summary>
    /// Тип отложенного бонуса.
    /// </summary>
    public DelayedBonusType? DelayedBonusType { get; set; }

    /// <summary>
    /// Цена посылки.
    /// </summary>
    public decimal? ParcelPrice { get; set; }

    /// <summary>
    /// Ключ товара.
    /// </summary>
    public int? CommodityKey { get; set; }

    /// <summary>
    /// Ключ оплаты товара.
    /// </summary>
    public int? PaymentOfCommodityKey { get; set; }

    /// <summary>
    /// Ключ набора товаров.
    /// </summary>
    public int? SetOfGoodsKey { get; set; }

    /// <summary>
    /// Ключ вендора.
    /// </summary>
    public int? VendorKey { get; set; }

    /// <summary>
    /// Температура.
    /// </summary>
    public float? Temperature { get; set; }

    /// <summary>
    /// Плотность.
    /// </summary>
    public float? Density { get; set; }

    /// <summary>
    /// GUID товара.
    /// </summary>
    public Guid? CommodityGuid { get; set; }

    /// <summary>
    /// GUID набора товаров.
    /// </summary>
    public Guid? SetOfGoodsGuid { get; set; }

    /// <summary>
    /// Начальная температура.
    /// </summary>
    public float? BeginTemperature { get; set; }

    /// <summary>
    /// Конечная температура.
    /// </summary>
    public float? EndTemperature { get; set; }

    /// <summary>
    /// Флаги продажи.
    /// </summary>
    public long? SellingFlags { get; set; }

    /// <summary>
    /// Перелив.
    /// </summary>
    public decimal? Overflow { get; set; }

    /// <summary>
    /// Тип карты.
    /// </summary>
    public DiscountApplicationTypeEnum? CardType { get; set; }

    /// <summary>
    /// Внешний код.
    /// </summary>
    public string? ExternalCode { get; set; }
}
