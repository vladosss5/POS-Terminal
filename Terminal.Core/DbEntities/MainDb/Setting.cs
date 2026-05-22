using Terminal.Core.Enums;

namespace Terminal.Core.DbEntities.MainDb;

public partial class Setting
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
