using System;
using System.Collections.Generic;

namespace Terminal.Core.DbEntities;

public partial class TransferCard
{
    /// <summary>
    /// Уникальный ключ перевода карты (первичный ключ)
    /// </summary>
    public int TransferCardKey { get; set; }

    /// <summary>
    /// Графический номер карты
    /// </summary>
    public decimal? GraphicalNumber { get; set; }

    /// <summary>
    /// Электронный номер карты
    /// </summary>
    public decimal? ElectronicNumber { get; set; }

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
    /// Период действия
    /// </summary>
    public long? ValidityPeriod { get; set; }

    /// <summary>
    /// Общий ID приложения
    /// </summary>
    public int? CommonApplicationId { get; set; }

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
    /// Ключ коллекции
    /// </summary>
    public int? CollectionKey { get; set; }

    /// <summary>
    /// Код ресурса
    /// </summary>
    public int? ResourceCode { get; set; }

    /// <summary>
    /// ID приложения
    /// </summary>
    public int? ApplicationId { get; set; }

    /// <summary>
    /// Цена посылки
    /// </summary>
    public decimal? ParcelPrice { get; set; }

    /// <summary>
    /// Связанная организация
    /// </summary>
    public virtual ListOrg Organisation { get; set; } = null!;
}
