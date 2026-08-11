using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Background;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Entities.Models;
using Terminal.Core.Enums;
using Terminal.Core.Interfaces;

namespace Terminal.Application.Services;

/// <summary>
/// Фоновый сервис проверки обновлений.
/// </summary>
public class UpgradeBackgroundService : IUpgradeBackgroundService
{
    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger<UpgradeBackgroundService> _logger;

    /// <inheritdoc cref="IUpdateInstallerService" />
    private readonly IUpdateInstallerService _updateInstallerService;

    /// <inheritdoc cref="IStatusNotifierService" />
    private readonly IStatusNotifierService _statusNotifierService;

    /// <inheritdoc cref="IPopupService" />
    private readonly IPopupService _popupService;

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
    /// Идёт ли загрузка в текущий момент?
    /// </summary>
    private bool _downloadIsInProgress;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public UpgradeBackgroundService(
        ILogger<UpgradeBackgroundService> logger, 
        IUpdateInstallerService updateInstallerService, 
        IStatusNotifierService statusNotifierService, 
        IPopupService popupService)
    {
        _logger = logger;
        _updateInstallerService = updateInstallerService;
        _statusNotifierService = statusNotifierService;
        _popupService = popupService;
    }

    /// <summary>
    /// Основной метод запуска действий.
    /// </summary>
    public async Task StartAutoUpgradeAsync()
    {
        _logger.LogInformation("Update check service started");
        
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));
        do
        {
            await CheckAndDownloadUpdateAsync();
        }
        while (await timer.WaitForNextTickAsync() && !_downloadIsInProgress);
    }

    /// <summary>
    /// Проверить наличие и загрузить обновление из TMS.
    /// </summary>
    private async Task CheckAndDownloadUpdateAsync()
    {
        _logger.LogInformation("Checking for updates...");

        if (!await _updateInstallerService.CheckForUpdates())
            return;

        _logger.LogInformation("New version found. Downloading...");
        UpdateDownloadingStatus(DownloadStatus.InProcess);
        _downloadIsInProgress = true;

        await _updateInstallerService.DownloadUpdatingFileAsync();

        _logger.LogInformation("Update downloaded successfully");
        UpdateDownloadingStatus(DownloadStatus.Completed);
            
        _downloadIsInProgress = false;

        _ = Task.Run(async () =>
        {
            await Task.Delay(10000);
            UpdateDownloadingStatus(DownloadStatus.NotFound);
        });
    }

    /// <summary>
    /// Обновить статус скачивания обновлений.
    /// </summary>
    /// <param name="downloadingStatus">Статус скачивания.</param>
    private void UpdateDownloadingStatus(DownloadStatus downloadingStatus)
    {
        var status = new Status { Type = StatusType.UpdatePatch };
        
        if (downloadingStatus == DownloadStatus.NotFound)
        {
            _statusNotifierService.RemoveStatusByType(status.Type);
        }
        else
        {
            status.IconName = downloadingStatus switch
            {
                DownloadStatus.InProcess => DownloadIconName,
                DownloadStatus.Aborted => AbortedIconName,
                DownloadStatus.Completed => CompletedIconName,
                _ => status.IconName
            };

            _statusNotifierService.AddOrChangeStatus(status);    
        }
        
        _statusNotifierService.Notify();
    }
}