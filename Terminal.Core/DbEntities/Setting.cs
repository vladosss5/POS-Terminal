using System;
using System.Collections.Generic;

namespace Terminal.Core.DbEntities;

public partial class Setting
{
    /// <summary>
    /// Первичный ключ настройки
    /// </summary>
    public int SettingsKey { get; set; }

    /// <summary>
    /// Значение настройки
    /// </summary>
    public int? Value { get; set; }
}
