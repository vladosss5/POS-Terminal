using Terminal.Core.Enums;

namespace Terminal.Core.Entities.DbEntities.MainDb;

public class Setting
{
    /// <summary>
    /// Первичный ключ настройки
    /// </summary>
    public SettingsKey? SettingsKey { get; set; }

    /// <summary>
    /// Значение настройки
    /// </summary>
    public int? Value { get; set; }
}
