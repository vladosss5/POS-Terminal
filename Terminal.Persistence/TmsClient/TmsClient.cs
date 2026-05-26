using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Terminal.Core.Enums;
using Terminal.Core.Models;

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

    /// <inheritdoc/>
    public async Task<bool> SendEncashmentTablesAsync(byte[] data, TableToSendDto table, int batchNumber, int recordCount)
    {
        const int maxRetries = 3;
        var attempt = 0;

        while (true)
        {
            try
            {
                var metadata = new Dictionary<string, string>
                {
                    ["TableName"] = table.Name,
                    ["DisplayName"] = table.DisplayName,
                    ["BatchNumber"] = batchNumber.ToString(),
                    ["RecordCount"] = recordCount.ToString(),
                    ["DatabaseName"] = table.DbName,
                    ["Timestamp"] = DateTime.UtcNow.ToString("O"),
                    ["ContentType"] = "application/json+gzip"
                };
            
                using var content = new MultipartFormDataContent();

                var fileContent = new ByteArrayContent(data);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/gzip");
            
                content.Add(
                    fileContent, "file", $"encashment_{metadata["TableName"]}_batch_{metadata["BatchNumber"]}.json.gz");

                foreach (var kvp in metadata)
                    content.Add(new StringContent(kvp.Value), kvp.Key);

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _jwt);

                var response = await _httpClient.PostAsync("/Encashment/upload", content);
                if (response.StatusCode == HttpStatusCode.OK)
                {

                }
                
                return true;
            }
            catch (Exception e)
            {
                attempt++;

                if (attempt >= maxRetries)
                    return false;
            }
        }
    }
}