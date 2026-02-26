using System;
using System.Collections.Generic;

namespace Terminal.Core.DbEntities;

public partial class SellingCoupon
{
    /// <summary>
    /// Первичный ключ продажи купона (по магазину)
    /// </summary>
    public int SellingCouponShopKey { get; set; }

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
    /// Графический номер купона
    /// </summary>
    public string? GraphicalNumber { get; set; }

    /// <summary>
    /// Электронный номер
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
    /// Использованный объём
    /// </summary>
    public int? UsedVolume { get; set; }

    /// <summary>
    /// Ключ корзины
    /// </summary>
    public int? ShoppingCartKey { get; set; }

    /// <summary>
    /// Ключ ресурса (топлива)
    /// </summary>
    public int? ResourceKey { get; set; }

    /// <summary>
    /// Ключ коллекции
    /// </summary>
    public int? CollectionKey { get; set; }

    /// <summary>
    /// Тип купона
    /// </summary>
    public int? CouponType { get; set; }

    /// <summary>
    /// Ключ организации
    /// </summary>
    public int? OrganisationKey { get; set; }

    /// <summary>
    /// Ключ персоны
    /// </summary>
    public int? PersonKey { get; set; }

    /// <summary>
    /// GUID товара
    /// </summary>
    public Guid? CommodityGuid { get; set; }

    /// <summary>
    /// GUID набора товаров
    /// </summary>
    public Guid? SetOfGoodsGuid { get; set; }

    /// <summary>
    /// Код ошибки
    /// </summary>
    public int? ErrorCode { get; set; }
}
