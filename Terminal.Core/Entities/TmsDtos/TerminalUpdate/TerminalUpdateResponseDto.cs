namespace Terminal.Core.Entities.TmsDtos.TerminalUpdate;

/// <summary>
/// ДТО ответа обновления терминала.
/// </summary>
public class TerminalUpdateResponseDto
{
    /// <summary>
    /// Номер настройки в БД.
    /// </summary>
    public int PosSettingsKey { get; set; }
    
    /// <summary>
    /// Base64 строка с данными.
    /// </summary>
    public string? Value { get; set; }
}