using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Entities.Models;
using Terminal.Core.Entities.TmsDtos.TerminalUpdate;
using Terminal.Core.Enums;
using Terminal.Core.Exceptions;
using Terminal.Core.Interfaces;

namespace Terminal.Application.Services;

public class TmsService : ITmsService
{
    /// <summary>
    /// Сервис логирования.
    /// </summary>
    private readonly ILogger<TmsService> _logger;
    
    /// <inheritdoc cref="ITmsClient" />
    private readonly ITmsClient _tmsClient;
    
    /// <inheritdoc cref="IParameterService" />
    private readonly IParameterService _parameterService;
    
    ///<inheritdoc cref="IConfigurationService"/>
    private readonly IConfigurationService _configurationService;
    
    /// <inheritdoc cref="ICryptographyService" />
    private readonly ICryptographyService _cryptographyService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public TmsService(
        ILogger<TmsService> logger,
        ITmsClient tmsClient, 
        IParameterService parameterService, 
        IConfigurationService configurationService, 
        ICryptographyService cryptographyService)
    {
        _tmsClient = tmsClient;
        _parameterService = parameterService;
        _configurationService = configurationService;
        _cryptographyService = cryptographyService;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<TerminalUpdateResponseDto?> GetConfigurationAsync(SettingsType settingType)
    {
        await CheckAndAuthAsync();
        
        var response = await _tmsClient.GetAsync($"/terminal-updating/updates/{(int)settingType}");
        if (!response.IsSuccessStatusCode)
            return null;
        
        var result = await response.Content.ReadFromJsonAsync<TerminalUpdateResponseDto>();
        
        return result;
    }

    /// <inheritdoc/>
    public async Task SendConfirmationUpdatingAsync(int[] updatedSettingIds)
    {
        await CheckAndAuthAsync();
        
        var json = JsonSerializer.Serialize(updatedSettingIds);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        await _tmsClient.PostAsync("/terminal-updating/confirmations", content);
    }

    /// <inheritdoc/>
    public async Task<byte[]> GetResultsEncashmentCollectionAsync()
    {
        await CheckAndAuthAsync();
        
        var response = await _tmsClient.GetAsync("/encashment/download-results");
        var result = await response.Content.ReadAsByteArrayAsync();
        
        return result;
    }

    /// <inheritdoc/>
    public async Task<bool> SendEncashmentTablesAsync(byte[] data, TableToSendDto table, string fileName, int recordCount)
    {
        await CheckAndAuthAsync();
        
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

                var response = await _tmsClient.PostAsync("/encashment/upload", content);
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
        await CheckAndAuthAsync();
        
        await _tmsClient.GetAsync("/encashment/start");
    }

    /// <inheritdoc/>
    public async Task<(Stream, string)> DownloadUpdatingFileAsync()
    {
        await CheckAndAuthAsync();
        
        var response = await _tmsClient.GetAsync($"terminal-updating/download_apk");

        if (!response.IsSuccessStatusCode)
            throw new NotFoundException("Обновлений не найдено");
        
        string? fileHash = null;
        if (response.Headers.TryGetValues("X-File-Hash", out var hashValues))
            fileHash = hashValues.FirstOrDefault();
        
        return (await response.Content.ReadAsStreamAsync(), fileHash!);
    }

    /// <inheritdoc/>
    public async Task ConfirmReceiptUpdatingFileAsync()
    {
        var response = await _tmsClient.GetAsync($"terminal-updating/confirm-receipt-apk");
        
        if (!response.IsSuccessStatusCode)
            throw new NotFoundException("Ошибка сервера.");
    }

    /// <inheritdoc/>
    public async Task<string?> GetNumberNewVersion()
    {
        var response = await _tmsClient.GetAsync($"terminal-updating/new-version");
        var newVersionInfo = await response.Content.ReadAsStringAsync();

        return newVersionInfo;
    }

    /// <summary>
    /// Проверить статус авторизации. Если не авторизован, то авторизоваться.
    /// </summary>
    private async Task CheckAndAuthAsync()
    {
        if (_tmsClient.ConnectionStatus == TmsConnectionStatus.Authorized)
            return;
        
        var terminalNumber = await _parameterService.GetValueAsync(AppParameter.SerialNO111);
        var plainText = terminalNumber + " " + Guid.NewGuid();
        var password = _configurationService.CurrentSetting.TmsConfiguration!.Key;
        var salt = _configurationService.CurrentSetting.TmsConfiguration!.Salt;
        var workload = _cryptographyService.EncryptAes(plainText, password, Encoding.UTF8.GetBytes(salt));

        await _tmsClient.AuthenticationAsync(workload);
    }
}