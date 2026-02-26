using System;
using System.Collections.Generic;

namespace Terminal.Core.DbEntities;

public partial class Correction
{
    /// <summary>
    /// Первичный ключ коррекции
    /// </summary>
    public int CorrectionsKey { get; set; }

    /// <summary>
    /// Ключ магазина
    /// </summary>
    public int? ShopKey { get; set; }

    /// <summary>
    /// Номер транзакции
    /// </summary>
    public int? TransactionNumber { get; set; }

    /// <summary>
    /// Ключ транзакции
    /// </summary>
    public int? TransactionKey { get; set; }

    /// <summary>
    /// Электронный номер
    /// </summary>
    public long? ElectronicNumber { get; set; }

    /// <summary>
    /// Тип приложения
    /// </summary>
    public byte? ApplicationType { get; set; }

    /// <summary>
    /// Тип коррекции
    /// </summary>
    public byte? CorrectionType { get; set; }

    /// <summary>
    /// Тип параметра
    /// </summary>
    public byte? ParameterType { get; set; }

    /// <summary>
    /// Заменяемое значение параметра
    /// </summary>
    public string? ParameterRepValue { get; set; }

    /// <summary>
    /// Добавляемое значение параметра
    /// </summary>
    public decimal? ParameterAddValue { get; set; }

    /// <summary>
    /// Дата внесения
    /// </summary>
    public DateTime? EnterDate { get; set; }

    /// <summary>
    /// Дата начала
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Дата окончания
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Ключ эмитента
    /// </summary>
    public int? IssuerKey { get; set; }

    /// <summary>
    /// Ключ организации
    /// </summary>
    public int? OrganisationKey { get; set; }

    /// <summary>
    /// Ключ владельца
    /// </summary>
    public int? OwnerKey { get; set; }

    /// <summary>
    /// Номер приложения
    /// </summary>
    public int? ApplicationNumber { get; set; }

    /// <summary>
    /// Признак удаления
    /// </summary>
    public bool? IsDelete { get; set; }

    /// <summary>
    /// Примечание
    /// </summary>
    public string? Note { get; set; }
}
