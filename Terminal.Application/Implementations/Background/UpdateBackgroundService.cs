using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;

namespace Terminal.Application.Implementations.Background;

/// <summary>
/// Фоновый сервис проверки обновлений.
/// </summary>
public class UpdateBackgroundService : BackgroundService
{
    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger<UpdateBackgroundService> _logger;

    /// <inheritdoc cref="IUpdateInstallerService" />
    private readonly IUpdateInstallerService _updateInstallerService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public UpdateBackgroundService(
        ILogger<UpdateBackgroundService> logger, 
        IUpdateInstallerService updateInstallerService)
    {
        _logger = logger;
        _updateInstallerService = updateInstallerService;
    }

    /// <summary>
    /// Основной метод запуска действий.
    /// </summary>
    /// <param name="stoppingToken">Токен завершения.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Update check service started");
        
        await CheckAndDownloadUpdateAsync(stoppingToken);

        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await CheckAndDownloadUpdateAsync(stoppingToken);
        }
    }
    
    /// <summary>
    /// Проверить наличие и загрузить обновление из TMS.
    /// </summary>
    /// <param name="token">Токен отмены.</param>
    private async Task CheckAndDownloadUpdateAsync(CancellationToken token)
    {
        try
        {
            _logger.LogInformation("Checking for updates...");

            if (await _updateInstallerService.CheckForUpdates())
            {
                _logger.LogInformation("New version found. Downloading...");
                await _updateInstallerService.DownloadUpdatingFileAsync();
                _logger.LogInformation("Update downloaded successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking for updates");
        }
    }
}