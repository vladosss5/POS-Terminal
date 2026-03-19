using Terminal.Application.Interfaces.Services;
using Terminal.Core.Models;

namespace Terminal.Application.Implementations.Services;

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
        throw new NotImplementedException();
    }

    public event EventHandler<string>? StatusChanged;
}