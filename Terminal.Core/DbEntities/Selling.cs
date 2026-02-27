using System;
using System.Collections.Generic;

namespace Terminal.Core.DbEntities;

public partial class Selling
{
    /// <summary>
    /// Первичный ключ продажи (по магазину)
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
    public int? Amount { get; set; }

    /// <summary>
    /// Стоимость по магазину
    /// </summary>
    public int? ShopCost { get; set; }

    /// <summary>
    /// Базовая стоимость по магазину
    /// </summary>
    public int? ShopBaseCost { get; set; }

    /// <summary>
    /// Цена продажи
    /// </summary>
    public int? SellingPrice { get; set; }

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
    /// Подпись
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
    public int? AppLimit { get; set; }

    /// <summary>
    /// Значение приложения
    /// </summary>
    public double? AppValue { get; set; }

    /// <summary>
    /// Второй лимит приложения
    /// </summary>
    public double? AppSecondLimit { get; set; }

    /// <summary>
    /// Второе значение приложения
    /// </summary>
    public double? AppSecondValue { get; set; }

    /// <summary>
    /// Номер чека
    /// </summary>
    public int? CheckNumber { get; set; }

    /// <summary>
    /// Период валидности
    /// </summary>
    public long? ValidityPeriod { get; set; }

    /// <summary>
    /// Общий ID приложения
    /// </summary>
    public int? CommonApplicationId { get; set; }

    /// <summary>
    /// GUID транзакции
    /// </summary>
    public Guid? Guid { get; set; }

    /// <summary>
    /// Код ошибки
    /// </summary>
    public int? ErrorCode { get; set; }

    /// <summary>
    /// Бонусы начисленные
    /// </summary>
    public double? BonusIn { get; set; }

    /// <summary>
    /// Бонусы списанные
    /// </summary>
    public int? BonusOut { get; set; }

    /// <summary>
    /// Стоимость начисленных бонусов
    /// </summary>
    public int? BonusInCost { get; set; }

    /// <summary>
    /// Стоимость списанных бонусов
    /// </summary>
    public int? BonusOutCost { get; set; }

    /// <summary>
    /// Признак отчёта по счёту
    /// </summary>
    public int? IsAccountRep { get; set; }

    /// <summary>
    /// Запрошенный объём
    /// </summary>
    public int? RequestedAmount { get; set; }

    /// <summary>
    /// Запрошенная стоимость
    /// </summary>
    public int? RequestedCost { get; set; }

    /// <summary>
    /// Базовая цена
    /// </summary>
    public int? BasePrice { get; set; }

    /// <summary>
    /// Стоимость для клиента
    /// </summary>
    public int? ClientCost { get; set; }

    /// <summary>
    /// Флаги запроса
    /// </summary>
    public int? RequestFlags { get; set; }

    /// <summary>
    /// Тип отложенного бонуса
    /// </summary>
    public int? DelayedBonusType { get; set; }

    /// <summary>
    /// Цена посылки
    /// </summary>
    public int? ParcelPrice { get; set; }

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
    public int? Temperature { get; set; }

    /// <summary>
    /// Плотность
    /// </summary>
    public int? Density { get; set; }

    /// <summary>
    /// GUID товара
    /// </summary>
    public Guid? CommodityGuid { get; set; }

    /// <summary>
    /// GUID набора товаров
    /// </summary>
    public Guid? SetOfGoodsGuid { get; set; }

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
    /// Перелив
    /// </summary>
    public decimal? Overflow { get; set; }

    /// <summary>
    /// Тип карты
    /// </summary>
    public int? CardType { get; set; }

    /// <summary>
    /// Внешний код
    /// </summary>
    public string? ExternalCode { get; set; }
}
