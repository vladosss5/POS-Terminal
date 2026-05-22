using System.Text;
using System.Text.Json;
using Terminal.Core.Enums;
using Terminal.Core.Models;

namespace Terminal.Persistence.TmsClient;

public class TmsClient : ITmsClient
{
    private readonly HttpClient _httpClient;
    
    private string? _jwt;

    public TmsConnectionStatus ConnectionStatus { get; private set; }

    public TmsClient(string addressBase)
    {
        ConnectionStatus = TmsConnectionStatus.Disconnected;
        
        var socketsHandler = new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(2) };
        _httpClient = new HttpClient(socketsHandler);
        _httpClient.BaseAddress = new Uri(addressBase);
    }
    
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