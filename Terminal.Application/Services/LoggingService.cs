using MainHelpers.Logger;
using Terminal.Core.Interfaces;

namespace Terminal.Application.Services;

/// <inheritdoc/>
public class LoggingService : ILoggingService
{
    /// <inheritdoc cref="LoggerClass" />   
    private readonly LoggerClass _sncLogger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public LoggingService(LoggerClass sncLogger)
    {
        _sncLogger = sncLogger;
    }

    /// <inheritdoc/>
    public void LogInformation(string message, bool withAdditional = true)
    {
        _sncLogger.WriteLog(message, MessageTypes.Message, withAdditional);
    }

    /// <inheritdoc/>
    public void LogError(string message, bool withAdditional = true)
    {
        _sncLogger.WriteLog(message, MessageTypes.Error, withAdditional);
    }

    /// <inheritdoc/>
    public void LogDebug(string message, bool withAdditional = true)
    {
#if DEBUG
        _sncLogger.WriteLog(message, MessageTypes.Message, withAdditional);  
#endif
    }

    /// <inheritdoc/>
    public void LogWarning(string message, bool withAdditional = true)
    {
        _sncLogger.WriteLog(message, MessageTypes.Warning, withAdditional);
    }

    /// <inheritdoc/>
    public void LogData(string message, bool withAdditional = true)
    {
        _sncLogger.WriteLog(message, MessageTypes.Data, withAdditional);
    }

    /// <inheritdoc/>
    public void LogClient(string message, bool withAdditional = true)
    {
        _sncLogger.WriteLog(message, MessageTypes.Client, withAdditional);
    }
}