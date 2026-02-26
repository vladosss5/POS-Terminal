using System;
using System.Collections.Generic;

namespace Terminal.Core.DbEntities;

public partial class Dispenser
{
    /// <summary>
    /// Первичный ключ диспенсера (по магазину)
    /// </summary>
    public int DispenserShopKey { get; set; }

    /// <summary>
    /// Ключ поставщика / вендора
    /// </summary>
    public int? VendorKey { get; set; }

    /// <summary>
    /// Ключ ресурса
    /// </summary>
    public int? ResourceKey { get; set; }

    /// <summary>
    /// Ключ ресурса по магазину
    /// </summary>
    public int? ResourceShopKey { get; set; }

    /// <summary>
    /// Начальный остаток
    /// </summary>
    public decimal? BeginBalance { get; set; }

    /// <summary>
    /// Конечный остаток
    /// </summary>
    public decimal? EndBalance { get; set; }

    /// <summary>
    /// Начальный остаток (расчётный)
    /// </summary>
    public decimal? BeginBalanceCalculation { get; set; }

    /// <summary>
    /// Конечный остаток (расчётный)
    /// </summary>
    public decimal? EndBalanceCalculation { get; set; }

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

    /// <summary>
    /// Флаги
    /// </summary>
    public long? Flags { get; set; }

    /// <summary>
    /// Наименование вендора
    /// </summary>
    public string? VendorName { get; set; }

    /// <summary>
    /// Начальная температура
    /// </summary>
    public decimal? BeginTemperature { get; set; }

    /// <summary>
    /// Конечная температура
    /// </summary>
    public decimal? EndTemperature { get; set; }
}
