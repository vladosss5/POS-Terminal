using Terminal.Core.Models;

namespace Terminal.Application.Interfaces.Services;

public interface ITmsClient
{
    /// <summary>
    /// Подключиться к серверу
    /// </summary>
    public Task<bool> ConnectAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Отключиться
    /// </summary>
    public Task DisconnectAsync();
    
    /// <summary>
    /// Авторизация на сервере
    /// </summary>
    public Task<AuthorizationResult> AuthorizeAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получить обновления таблиц (справочники)
    /// </summary>
    public Task<ReceiveResult> ReceiveTablesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получить обновления ПО
    /// </summary>
    public Task<ReceiveResult> ReceiveUpdatesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Отправить транзакции на сервер
    /// </summary>
    public Task<SendTableResult> SendTableAsync(string tableName, string keyField, byte[] data, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Отправить файл на сервер
    /// </summary>
    public Task<bool> SendFileAsync(string filePath, string fileName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Завершить сеанс связи
    /// </summary>
    public Task EndDialogAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Проверить соединение
    /// </summary>
    public bool IsConnected { get; }
}