using Terminal.Core.Enums;

namespace Terminal.Core.TmsDtos.TerminalUpdate;

/// <summary>
/// ДТО запроса на обновление терминала.
/// </summary>
public class TerminalUpdateRequestDto
{
    /// <summary>
    /// Номер терминала.
    /// </summary>
    public long TerminalId { get; set; }
    
    /// <summary>
    /// Тип настроек.
    /// </summary>
    public SettingsType SettingType { get; set; }
}