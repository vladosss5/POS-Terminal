namespace Terminal.Application.Interfaces.Services;

public interface ITmsConnectionService
{
    /// <summary>
    /// Уведомление о получении ответа от TMS.
    /// </summary>
    public event Action<ulong>? OnDataReceived;
    
    public Task<bool> ConnectAndAuthorizeAsync(ulong terminalId, string serverHost, int port, CancellationToken cancellationToken = default);
    public Task SendDataAsync(byte[] data, CancellationToken cancellationToken = default);
    public Task DisconnectAsync();
    public bool IsConnected { get; }
}