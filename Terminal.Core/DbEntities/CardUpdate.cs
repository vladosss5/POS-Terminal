using System;
using System.Collections.Generic;

namespace Terminal.Core.DbEntities;

public partial class CardUpdate
{
    /// <summary>
    /// Первичный ключ обновления карты
    /// </summary>
    public int CardUpdateKey { get; set; }

    /// <summary>
    /// Ключ магазина
    /// </summary>
    public int? ShopKey { get; set; }

    /// <summary>
    /// Ключ терминала
    /// </summary>
    public decimal? TerminalKey { get; set; }

    /// <summary>
    /// Ключ смены
    /// </summary>
    public int? ShiftKey { get; set; }

    /// <summary>
    /// Номер транзакции
    /// </summary>
    public int? TransactionNumber { get; set; }

    /// <summary>
    /// Электронный номер карты
    /// </summary>
    public long? ElectronicNumber { get; set; }

    /// <summary>
    /// Ключ транзакции
    /// </summary>
    public int? TransactionKey { get; set; }

    /// <summary>
    /// Код результата операции
    /// </summary>
    public byte? ResultCode { get; set; }

    /// <summary>
    /// Ключ эмитента
    /// </summary>
    public int? IssuerKey { get; set; }

    /// <summary>
    /// Тип коррекции
    /// </summary>
    public byte? CorrectionType { get; set; }

    /// <summary>
    /// Значение до изменения
    /// </summary>
    public string? BeforeValue { get; set; }

    /// <summary>
    /// Значение после изменения
    /// </summary>
    public string? AfterValue { get; set; }

    /// <summary>
    /// Дата обновления
    /// </summary>
    public DateTime? UpdateDate { get; set; }

    /// <summary>
    /// Код ошибки
    /// </summary>
    public int? ErrorCode { get; set; }

    /// <summary>
    /// Признак отправки в центральную систему
    /// </summary>
    public bool? IsSent { get; set; }

    /// <summary>
    /// Тип приложения
    /// </summary>
    public byte? ApplicationType { get; set; }

    /// <summary>
    /// Тип параметра
    /// </summary>
    public byte? ParameterType { get; set; }

    /// <summary>
    /// Дата внесения записи
    /// </summary>
    public DateTime? EnterDate { get; set; }

    /// <summary>
    /// Дата начала действия
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Дата окончания действия
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Ключ организации
    /// </summary>
    public int? OrganisationKey { get; set; }

    /// <summary>
    /// Ключ владельца
    /// </summary>
    public int? OwnerKey { get; set; }

    /// <summary>
    /// Ключ карты эмитента
    /// </summary>
    public int? IssuerCardKey { get; set; }

    /// <summary>
    /// Номер приложения
    /// </summary>
    public int? ApplicationNumber { get; set; }

    /// <summary>
    /// Заменяемое значение параметра
    /// </summary>
    public string? ParameterRepValue { get; set; }

    /// <summary>
    /// Добавляемое значение параметра
    /// </summary>
    public string? ParameterAddValue { get; set; }

    /// <summary>
    /// Ключ терминала смены
    /// </summary>
    public int? ShiftTerminalKey { get; set; }
}
