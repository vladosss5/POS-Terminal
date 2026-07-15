using Terminal.Core.Entities.Models;
using Terminal.Core.Enums;
using Terminal.Core.Interfaces;

namespace Terminal.Application.Services;

/// <summary>
/// Общая реализация-заглушка для сервиса по работе со считывателем карт.
/// </summary>
public class CommonCardReaderService : ICardReaderService
{
    public void Dispose()
    {
        // TODO release managed resources here
    }

    public Task<CardReadResult> ReadCardAsync(
        int timeoutSeconds = 30, 
        CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            Task.Delay(timeoutSeconds, cancellationToken);
            return new CardReadResult{};
        }, cancellationToken);
    }

    public event EventHandler<CardReaderStatus>? StatusChanged;
}