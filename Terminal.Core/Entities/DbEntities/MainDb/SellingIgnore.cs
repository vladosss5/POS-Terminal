namespace Terminal.Core.Entities.DbEntities.MainDb;

public class SellingIgnore
{
    /// <summary>
    /// Первичный ключ игнорируемой продажи (по магазину)
    /// </summary>
    public int TransactionShopKey { get; set; }

    /// <summary>
    /// Ключ смены
    /// </summary>
    public int? ShiftKey { get; set; }

    /// <summary>
    /// Ключ магазина
    /// </summary>
    public int? ShopKey { get; set; }

    /// <summary>
    /// Ключ терминала
    /// </summary>
    public decimal? TerminalKey { get; set; }

    /// <summary>
    /// Дата и время транзакции
    /// </summary>
    public DateTime? TransactionDatetime { get; set; }

    /// <summary>
    /// Графический номер
    /// </summary>
    public decimal? GraphicalNumber { get; set; }

    /// <summary>
    /// Электронный номер
    /// </summary>
    public decimal? ElectronicNumber { get; set; }

    /// <summary>
    /// Базовый тип
    /// </summary>
    public int? BaseType { get; set; }

    /// <summary>
    /// Производный тип
    /// </summary>
    public int? DerivedType { get; set; }

    /// <summary>
    /// Объём продажи
    /// </summary>
    public decimal? Amount { get; set; }

    /// <summary>
    /// Стоимость по магазину
    /// </summary>
    public decimal? ShopCost { get; set; }

    /// <summary>
    /// Базовая стоимость по магазину
    /// </summary>
    public decimal? ShopBaseCost { get; set; }

    /// <summary>
    /// Цена продажи
    /// </summary>
    public decimal? SellingPrice { get; set; }

    /// <summary>
    /// Ключ корзины
    /// </summary>
    public int? ShoppingCartKey { get; set; }

    /// <summary>
    /// Ключ ресурса
    /// </summary>
    public int? ResourceKey { get; set; }

    /// <summary>
    /// Ключ коллекции
    /// </summary>
    public int? CollectionKey { get; set; }

    /// <summary>
    /// Код ресурса (ссылка на ResourceCode.FuelCodeKey)
    /// </summary>
    public int? ResourceCode { get; set; }

    /// <summary>
    /// Ключ ресурса по магазину
    /// </summary>
    public int? ResourceShopKey { get; set; }

    /// <summary>
    /// Наименование ресурса
    /// </summary>
    public string? ResourceName { get; set; }

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
    /// Ключ персоны
    /// </summary>
    public int? PersonKey { get; set; }

    /// <summary>
    /// ID приложения
    /// </summary>
    public int? ApplicationId { get; set; }

    /// <summary>
    /// Количество транзакций
    /// </summary>
    public int? TransactionCount { get; set; }

    /// <summary>
    /// Подпись / сигнатура
    /// </summary>
    public string? Sign { get; set; }

    /// <summary>
    /// Статус приложения
    /// </summary>
    public int? AppStatus { get; set; }

    /// <summary>
    /// Режим приложения
    /// </summary>
    public int? AppMode { get; set; }

    /// <summary>
    /// Лимит приложения
    /// </summary>
    public decimal? AppLimit { get; set; }

    /// <summary>
    /// Значение приложения
    /// </summary>
    public decimal? AppValue { get; set; }

    /// <summary>
    /// Второй лимит приложения
    /// </summary>
    public decimal? AppSecondLimit { get; set; }

    /// <summary>
    /// Второе значение приложения
    /// </summary>
    public decimal? AppSecondValue { get; set; }

    /// <summary>
    /// Номер чека
    /// </summary>
    public int? CheckNumber { get; set; }

    /// <summary>
    /// Период валидности
    /// </summary>
    public int? ValidityPeriod { get; set; }

    /// <summary>
    /// Общий ID приложения
    /// </summary>
    public int? CommonApplicationId { get; set; }

    /// <summary>
    /// GUID транзакции
    /// </summary>
    public string? Guid { get; set; }

    /// <summary>
    /// Код ошибки
    /// </summary>
    public int? ErrorCode { get; set; }

    /// <summary>
    /// Бонусы начисленные
    /// </summary>
    public decimal? BonusIn { get; set; }

    /// <summary>
    /// Бонусы списанные
    /// </summary>
    public decimal? BonusOut { get; set; }

    /// <summary>
    /// Стоимость начисленных бонусов
    /// </summary>
    public decimal? BonusInCost { get; set; }

    /// <summary>
    /// Стоимость списанных бонусов
    /// </summary>
    public decimal? BonusOutCost { get; set; }

    /// <summary>
    /// Признак отчёта по счёту
    /// </summary>
    public int? IsAccountRep { get; set; }

    /// <summary>
    /// Запрошенный объём
    /// </summary>
    public decimal? RequestedAmount { get; set; }

    /// <summary>
    /// Запрошенная стоимость
    /// </summary>
    public decimal? RequestedCost { get; set; }

    /// <summary>
    /// Базовая цена
    /// </summary>
    public decimal? BasePrice { get; set; }

    /// <summary>
    /// Стоимость для клиента
    /// </summary>
    public decimal? ClientCost { get; set; }

    /// <summary>
    /// Флаги запроса
    /// </summary>
    public int? RequestFlags { get; set; }

    /// <summary>
    /// Тип отложенного бонуса
    /// </summary>
    public int? DelayedBonusType { get; set; }

    /// <summary>
    /// Цена посылки / упаковки
    /// </summary>
    public decimal? ParcelPrice { get; set; }

    /// <summary>
    /// Ключ товара
    /// </summary>
    public int? CommodityKey { get; set; }

    /// <summary>
    /// Ключ оплаты товара
    /// </summary>
    public int? PaymentOfCommodityKey { get; set; }

    /// <summary>
    /// Ключ набора товаров
    /// </summary>
    public int? SetOfGoodsKey { get; set; }

    /// <summary>
    /// Ключ вендора
    /// </summary>
    public int? VendorKey { get; set; }

    /// <summary>
    /// Температура
    /// </summary>
    public decimal? Temperature { get; set; }

    /// <summary>
    /// Плотность
    /// </summary>
    public decimal? Density { get; set; }

    /// <summary>
    /// GUID товара
    /// </summary>
    public string? CommodityGuid { get; set; }

    /// <summary>
    /// GUID набора товаров
    /// </summary>
    public string? SetOfGoodsGuid { get; set; }

    /// <summary>
    /// Начальная температура
    /// </summary>
    public decimal? BeginTemperature { get; set; }

    /// <summary>
    /// Конечная температура
    /// </summary>
    public decimal? EndTemperature { get; set; }

    /// <summary>
    /// Флаги продажи
    /// </summary>
    public long? SellingFlags { get; set; }

    /// <summary>
    /// Перелив / излишек
    /// </summary>
    public decimal? Overflow { get; set; }

    /// <summary>
    /// Тип карты
    /// </summary>
    public int? CardType { get; set; }
}
