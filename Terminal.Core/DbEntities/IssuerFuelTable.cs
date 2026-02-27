using System;
using System.Collections.Generic;

namespace Terminal.Core.DbEntities;

public partial class IssuerFuelTable
{
    /// <summary>
    /// Первичный ключ таблицы топлива эмитента
    /// </summary>
    public int IssuerFuelCodeKey { get; set; }

    /// <summary>
    /// ID эмитента
    /// </summary>
    public int? IssuerId { get; set; }

    /// <summary>
    /// Ключ ресурса по магазину
    /// </summary>
    public int? ResourceShopKey { get; set; }

    /// <summary>
    /// Ключ ресурса
    /// </summary>
    public int? ResourceKey { get; set; }

    /// <summary>
    /// Признак карты
    /// </summary>
    public byte? IsCard { get; set; }
}
