using System;
using System.Collections.Generic;

namespace Terminal.Core.DbEntities;

public partial class PosUpdate
{
    /// <summary>
    /// Первичный ключ обновления POS (по магазину)
    /// </summary>
    public int PosUpdateShopKey { get; set; }

    /// <summary>
    /// Дата обновления POS
    /// </summary>
    public DateTime? PosUpdateDate { get; set; }

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
    /// Электронный номер
    /// </summary>
    public long? ElectronicNumber { get; set; }

    /// <summary>
    /// Графический номер
    /// </summary>
    public long? GraphicalNumber { get; set; }

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
    /// Ключ коллекции
    /// </summary>
    public int? CollectionKey { get; set; }

    /// <summary>
    /// Ключ ресурса
    /// </summary>
    public int? ResourceKey { get; set; }

    /// <summary>
    /// GUID обновления
    /// </summary>
    public string? Guid { get; set; }

    /// <summary>
    /// Значение до изменения
    /// </summary>
    public string? BeforeValue { get; set; }

    /// <summary>
    /// Значение после изменения
    /// </summary>
    public string? AfterValue { get; set; }

    /// <summary>
    /// Тип обновления POS
    /// </summary>
    public byte? PosUpdateType { get; set; }

    /// <summary>
    /// Статус приложения
    /// </summary>
    public byte? AppStatus { get; set; }

    /// <summary>
    /// ID приложения
    /// </summary>
    public int? ApplicationId { get; set; }

    /// <summary>
    /// Изменённое значение
    /// </summary>
    public string? ChangeValue { get; set; }

    /// <summary>
    /// Номер чека
    /// </summary>
    public int? CheckNumber { get; set; }
}
