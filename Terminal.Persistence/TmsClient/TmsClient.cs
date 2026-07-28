using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
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

    /// <inheritdoc cref="IParameterService"/>
    private readonly IParameterService _parameterService;

    /// <summary>
    /// Http клиент.
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Токен авторизации.
    /// </summary>
    private string? _jwt;

    /// <summary>
    /// Адрес уже актуализирован из параметров.
    /// </summary>
    private bool _baseAddressResolved;

    /// <summary>
    /// Синхронизация ленивой инициализации адреса.
    /// </summary>
    private readonly SemaphoreSlim _addressLock = new(1, 1);

    /// <inheritdoc/>
    public TmsConnectionStatus ConnectionStatus { get; private set; }

    /// <summary>
    /// Конструктор.
    /// </summary>
    public TmsClient(
        IParameterService parameterService,
        ILogger<TmsClient> logger)
    {
        _parameterService = parameterService;
        _logger = logger;
        ConnectionStatus = TmsConnectionStatus.Disconnected;

        var socketsHandler = new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(2)
        };
        _httpClient = new HttpClient(socketsHandler)
        {
            BaseAddress = new Uri("http://127.0.0.1:5297/")
        };
    }

    /// <inheritdoc/>
    public void ChangeBaseAddress(string address)
    {
        if (string.IsNullOrEmpty(address))
            return;

        var oldAddress = _httpClient.BaseAddress?.ToString();
        _httpClient.BaseAddress = new Uri(address);
        _baseAddressResolved = true;

        _logger.LogInformation("TMS client base address has been changed. {Old} -> {New}",
            oldAddress, _httpClient.BaseAddress);
    }

    /// <inheritdoc/>
    public async Task AuthenticationAsync(string authData)
    {
        await EnsureBaseAddressAsync().ConfigureAwait(false);

        var json = JsonSerializer.Serialize(authData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("authentication", content).ConfigureAwait(false);
        _jwt = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwt);

        if (!string.IsNullOrEmpty(_jwt))
            ConnectionStatus = TmsConnectionStatus.Authorized;
    }

    /// <inheritdoc/>
    public async Task<HttpResponseMessage> GetAsync(string path)
    {
        await EnsureBaseAddressAsync().ConfigureAwait(false);
        return await _httpClient.GetAsync(path).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<HttpResponseMessage> PostAsync(string path, StringContent content)
    {
        await EnsureBaseAddressAsync().ConfigureAwait(false);
        return await _httpClient.PostAsync(path, content).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<HttpResponseMessage> PostAsync(string path, MultipartFormDataContent content)
    {
        await EnsureBaseAddressAsync().ConfigureAwait(false);
        return await _httpClient.PostAsync(path, content).ConfigureAwait(false);
    }

    /// <summary>
    /// Лениво подставляет IP/порт TMS из параметров БД.
    /// </summary>
    private async Task EnsureBaseAddressAsync()
    {
        if (_baseAddressResolved)
            return;

        await _addressLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (_baseAddressResolved)
                return;

            var ip = await _parameterService.GetValueAsync(AppParameter.TmsIp).ConfigureAwait(false);
            var port = await _parameterService.GetValueAsync(AppParameter.TmsPort).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(ip))
                ip = "127.0.0.1";
            if (string.IsNullOrWhiteSpace(port))
                port = "5297";

            var address = $"http://{ip}:{port}/";
            _httpClient.BaseAddress = new Uri(address);
            _baseAddressResolved = true;

            _logger.LogInformation("TMS client base address resolved to {Address}", address);
        }
        finally
        {
            _addressLock.Release();
        }
    }
}