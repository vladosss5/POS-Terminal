namespace Terminal.Core.Models;

/// <summary>
/// Конфигурирование подключения к TMS.
/// </summary>
public class TmsConfiguration
{
    /// <summary>
    /// Ключ.
    /// </summary>
    public string Key { get; init; } = string.Empty;
    
    /// <summary>
    /// Соль.
    /// </summary>
    public string Salt { get; init; } = string.Empty;
}