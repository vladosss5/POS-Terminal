using Terminal.Core.Models;

namespace Terminal.Application.Interfaces.Services;

/// <summary>
/// Сервис для работы со считывателем карт.
/// </summary>
public interface ICardReaderService : IDisposable
{
    /// <summary>
    /// Считать карту с ожиданием.
    /// </summary>
    /// <param name="timeoutSeconds">Тайм-аут ожидания в секундах.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Результат считывания.</returns>
    Task<CardReadResult> ReadCardAsync(
        int timeoutSeconds = 30, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Событие изменения статуса.
    /// </summary>
    event EventHandler<CardReaderStatus>? StatusChanged;
}