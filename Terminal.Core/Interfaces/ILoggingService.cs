namespace Terminal.Core.Interfaces;

/// <summary>
/// Сервис логирования.
/// </summary>
public interface ILoggingService
{
    /// <summary>
    /// Лог служебной информации.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    /// <param name="withAdditional">Включить в лог дополнение?</param>
    public void LogInformation(string message, bool withAdditional = true);

    /// <summary>
    /// Лог информации об ошибке.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    /// <param name="withAdditional">Включить в лог дополнение?</param>
    public void LogError(string message, bool withAdditional = true);

    /// <summary>
    /// Лог служебной информации при отладке.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    /// <param name="withAdditional">Включить в лог дополнение?</param>
    public void LogDebug(string message, bool withAdditional = true);
    
    /// <summary>
    /// Лог о предупреждение.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    /// <param name="withAdditional">Включить в лог дополнение?</param>
    public void LogWarning(string message, bool withAdditional = true);
    
    /// <summary>
    /// Лог информации с данными.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    /// <param name="withAdditional">Включить в лог дополнение?</param>
    public void LogData(string message, bool withAdditional = true);
    
    /// <summary>
    /// Лог информации о действие клиента.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    /// <param name="withAdditional">Включить в лог дополнение?</param>
    public void LogClient(string message, bool withAdditional = true);
}