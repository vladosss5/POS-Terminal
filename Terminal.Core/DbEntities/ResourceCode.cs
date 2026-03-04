using System;
using System.Collections.Generic;

namespace Terminal.Core.DbEntities;

public partial class ResourceCode
{
    /// <summary>
    /// Первичный ключ кода ресурса (топлива)
    /// </summary>
    public int FuelCodeKey { get; set; }

    /// <summary>
    /// Ключ коллекции ресурсов
    /// </summary>
    public int? CollectionKey { get; set; }

    /// <summary>
    /// Ключ ресурса
    /// </summary>
    public int? ResourceKey { get; set; }

    /// <summary>
    /// Наименование ресурса
    /// </summary>
    public string? ResourceName { get; set; }

    /// <summary>
    /// Цена ресурса
    /// </summary>
    public decimal? ResourcePrice { get; set; }

    /// <summary>
    /// Признак отображения в интерфейсе
    /// </summary>
    public byte? IsShow { get; set; }

    /// <summary>
    /// Плотность топлива
    /// </summary>
    public double? Density { get; set; }

    /// <summary>
    /// Температура
    /// </summary>
    public int? Temperature { get; set; }
}
