namespace Terminal.Core.Enums;

/// <summary>
/// Статусы подключения клиента к TMS.
/// </summary>
public enum TmsConnectionStatus
{
    /// <summary>
    /// Не подключен.
    /// </summary>
    Disconnected,

    /// <summary>
    /// Подключен.
    /// </summary>
    Connected,
    
    /// <summary>
    /// Аутентифицирован.
    /// </summary>
    Authorized
}