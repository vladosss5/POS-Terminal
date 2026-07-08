using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Enums;
using Terminal.Core.Models;

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

    /// <inheritdoc cref="IStatusNotifierService" />
    private readonly IStatusNotifierService _statusNotifierService;

    /// <summary>
    /// Название файла-иконки загрузки.
    /// </summary>
    private const string DownloadIconName = "downloading-file.png";
    
    /// <summary>
    /// Название файла-иконки ошибки загрузки.
    /// </summary>
    private const string AbortedIconName = "aborted.png";
    
    /// <summary>
    /// Название файла-иконки выполненной загрузки.
    /// </summary>
    private const string CompletedIconName = "done.png";

    /// <summary>
    /// Конструктор.
    /// </summary>
    public UpdateBackgroundService(
        ILogger<UpdateBackgroundService> logger, 
        IUpdateInstallerService updateInstallerService, 
        IStatusNotifierService statusNotifierService)
    {
        _logger = logger;
        _updateInstallerService = updateInstallerService;
        _statusNotifierService = statusNotifierService;
    }

    /// <summary>
    /// Основной метод запуска действий.
    /// </summary>
    /// <param name="stoppingToken">Токен завершения.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Update check service started");
        
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));
        do
        {
            await CheckAndDownloadUpdateAsync();
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    /// <summary>
    /// Проверить наличие и загрузить обновление из TMS.
    /// </summary>
    private async Task CheckAndDownloadUpdateAsync()
    {
        try
        {
            _logger.LogInformation("Checking for updates...");

            if (await _updateInstallerService.CheckForUpdates())
            {
                _logger.LogInformation("New version found. Downloading...");
                UpdateDownloadingStatus(DownloadStatus.InProcess);
                
                await _updateInstallerService.DownloadUpdatingFileAsync();
                
                _logger.LogInformation("Update downloaded successfully");
                UpdateDownloadingStatus(DownloadStatus.Completed);
            }
            else
            {
                _logger.LogInformation("New version not found.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking for updates");
            UpdateDownloadingStatus(DownloadStatus.Aborted);
        }
    }

    /// <summary>
    /// Обновить статцс скачивания обновлений.
    /// </summary>
    /// <param name="downloadingStatus">Статус скачивания.</param>
    private void UpdateDownloadingStatus(DownloadStatus downloadingStatus)
    {
        var status = new Status
        {
            Type = StatusType.UpdatePatch
        };

        status.IconName = downloadingStatus switch
        {
            DownloadStatus.InProcess => DownloadIconName,
            DownloadStatus.Aborted => AbortedIconName,
            DownloadStatus.Completed => CompletedIconName,
            _ => status.IconName
        };

        _statusNotifierService.AddOrChangeStatus(status);
        _statusNotifierService.Notify();
    }
}