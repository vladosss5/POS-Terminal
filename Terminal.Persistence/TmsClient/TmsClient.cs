using System.Text;
using System.Text.Json;
using Terminal.Core.Enums;

namespace Terminal.Persistence.TmsClient;

/// <inheritdoc/>
public class TmsClient : ITmsClient
{
    /// <summary>
    /// Http клиент. 
    /// </summary>
    private readonly HttpClient _httpClient;
    
    /// <summary>
    /// Токен авторизации.
    /// </summary>
    private string? _jwt;

    /// <inheritdoc/>
    public TmsConnectionStatus ConnectionStatus { get; private set; }

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="addressBase">Базовая часть адреса TMS.</param>
    public TmsClient(string addressBase)
    {
        ConnectionStatus = TmsConnectionStatus.Disconnected;
        
        var socketsHandler = new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(2) };
        _httpClient = new HttpClient(socketsHandler);
        _httpClient.BaseAddress = new Uri(addressBase);
    }
    
    /// <inheritdoc/>
    public async Task AuthenticationAsync(string authData)
    {
        var json = JsonSerializer.Serialize(authData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("Auth/Authentication", content);
        _jwt = await response.Content.ReadAsStringAsync();

        if (!string.IsNullOrEmpty(_jwt))
            ConnectionStatus = TmsConnectionStatus.Authorized;
    }
}