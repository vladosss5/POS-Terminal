using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Terminal.Core.Enums;
using Terminal.Core.Models;
using Terminal.Core.TmsDtos.TerminalUpdate;

namespace Terminal.Persistence.TmsClient;

/// <inheritdoc/>
public class TmsClient : ITmsClient
{
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
    /// <param name="addressBase">Базовая часть адреса TMS.</param>
    /// <param name="logger">Сервис логирования TMS клиента.</param>
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
    public async Task AuthenticationAsync(string authData)
    {
        try
        {
            var json = JsonSerializer.Serialize(authData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("authentication", content);
            _jwt = await response.Content.ReadAsStringAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwt);

            if (!string.IsNullOrEmpty(_jwt))
                ConnectionStatus = TmsConnectionStatus.Authorized;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<TerminalUpdateResponseDto?> GetConfigurationAsync(SettingsType settingType)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/terminal-updating/updates/{(int)settingType}");
            var result = await response.Content.ReadFromJsonAsync<TerminalUpdateResponseDto>();
            
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task SendConfirmationUpdatingAsync(int[] updatedSettingIds)
    {
        try
        {
            var json = JsonSerializer.Serialize(updatedSettingIds);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await _httpClient.PostAsync("/terminal-updating/confirmations", content);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<byte[]> GetResultsEncashmentCollectionAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/encashment/download-results");
            var result = await response.Content.ReadAsByteArrayAsync();
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> SendEncashmentTablesAsync(byte[] data, TableToSendDto table, string fileName, int recordCount)
    {
        const int maxRetries = 3;
        var attempt = 0;

        while (true)
        {
            try
            {
                var metadata = new Dictionary<string, string>
                {
                    ["TableName"] = table.Name.ToString(),
                    ["DisplayName"] = table.DisplayName,
                    ["RecordCount"] = recordCount.ToString(),
                    ["DatabaseName"] = table.DbName,
                    ["Timestamp"] = DateTime.UtcNow.ToString("O"),
                    ["ContentType"] = "application/json+gzip"
                };
            
                using var content = new MultipartFormDataContent();

                var fileContent = new ByteArrayContent(data);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/gzip");
            
                content.Add(fileContent, "file", fileName);

                foreach (var kvp in metadata)
                    content.Add(new StringContent(kvp.Value), kvp.Key);

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _jwt);

                var response = await _httpClient.PostAsync("/encashment/upload", content);
                if (response.StatusCode == HttpStatusCode.OK)
                {

                }
                
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"An attempt number: {attempt} to send a {table.Name} table package failed", e.Message);
                attempt++;

                if (attempt >= maxRetries)
                    return false;
            }
        }
    }

    /// <inheritdoc/>
    public async Task StartEncashmentOnTmsAsync()
    {
        await _httpClient.GetAsync("/encashment/start");
    }
}