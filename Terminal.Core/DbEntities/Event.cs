using System;
using System.Collections.Generic;

namespace Terminal.Core.DbEntities;

public partial class Event
{
    /// <summary>
    /// Первичный ключ события
    /// </summary>
    public int EventsKey { get; set; }

    /// <summary>
    /// Ключ терминала
    /// </summary>
    public decimal? TerminalKey { get; set; }

    /// <summary>
    /// Дата события
    /// </summary>
    public DateTime? EventDate { get; set; }

    /// <summary>
    /// Тип события
    /// </summary>
    public int? EventType { get; set; }

    /// <summary>
    /// Объект события
    /// </summary>
    public int? EventObject { get; set; }

    /// <summary>
    /// Результат события
    /// </summary>
    public int? EventResult { get; set; }

    /// <summary>
    /// Дополнительная информация о событии
    /// </summary>
    public string? EventInfo { get; set; }
}
