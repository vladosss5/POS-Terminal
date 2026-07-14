using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Terminal.Core.Enums;
using Terminal.Core.Interfaces;

namespace Terminal.Persistence.TmsClient;

/// <inheritdoc/>
public class TmsClient : ITmsClient
{
    /// <summary>
    /// Сервис логирования.
    /// </summary>
    private readonly ILogger<TmsClient> _logger;
    
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
    public TmsClient(
        string addressBase, 
        ILogger<TmsClient> logger)
    {
        _logger = logger;
        ConnectionStatus = TmsConnectionStatus.Disconnected;
        
        var socketsHandler = new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(2) };
        _httpClient = new HttpClient(socketsHandler);
        _httpClient.BaseAddress = new Uri(addressBase);
    }
    
    /// <inheritdoc/>
    public void ChangeBaseAddress(string address)
    {
        if (string.IsNullOrEmpty(address))
            return;
        
        var oldAddress = _httpClient.BaseAddress?.ToString();
        _httpClient.BaseAddress = new Uri(address);
        var newAddress = _httpClient.BaseAddress?.ToString();
        
        _logger.LogInformation($"TMS client base address has been changed. {oldAddress} -> {newAddress}");
    }
    
    /// <inheritdoc/>
    public async Task AuthenticationAsync(string authData)
    {
        var json = JsonSerializer.Serialize(authData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("authentication", content);
        _jwt = await response.Content.ReadAsStringAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwt);

        if (!string.IsNullOrEmpty(_jwt))
            ConnectionStatus = TmsConnectionStatus.Authorized;
    }

    /// <inheritdoc/>
    public async Task<HttpResponseMessage> GetAsync(string path)
    {
        var response = await _httpClient.GetAsync(path);
        return response;
    }

    /// <inheritdoc/>
    public async Task<HttpResponseMessage> PostAsync(string path, StringContent content)
    {
        var response = await _httpClient.PostAsync(path, content);
        return response;
    }

    /// <inheritdoc/>
    public async Task<HttpResponseMessage> PostAsync(string path, MultipartFormDataContent content)
    {
        var response = await _httpClient.PostAsync(path, content);
        return response;
    }
}