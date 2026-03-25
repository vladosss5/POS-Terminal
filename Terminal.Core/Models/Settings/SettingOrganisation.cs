namespace Terminal.Core.Models.Settings;

/// <summary>
/// Настройки организации для чеков.
/// </summary>
public class SettingOrganisation
{
    /// <summary>
    /// Сообщение вверху чека.
    /// </summary>
    public List<string>? Header { get; set; }
    
    /// <summary>
    /// Сообщение внизу чека.
    /// </summary>
    public List<string>? Footer { get; set; }
}