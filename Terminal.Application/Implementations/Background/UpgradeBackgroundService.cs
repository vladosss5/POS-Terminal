using System.Text;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Background;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Enums;
using Terminal.Core.Models;
using Terminal.Persistence.TmsClient;

namespace Terminal.Application.Implementations.Background;

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
    
    /// <inheritdoc cref="IParameterService" />
    private readonly IParameterService _parameterService;
    
    ///<inheritdoc cref="IConfigurationService"/>
    private readonly IConfigurationService _configurationService;
    
    /// <inheritdoc cref="ICryptographyService" />
    private readonly ICryptographyService _cryptographyService;
    
    /// <inheritdoc cref="ITmsClient" />
    private readonly ITmsClient _tmsClient;

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
    public UpgradeBackgroundService(
        ILogger<UpgradeBackgroundService> logger, 
        IUpdateInstallerService updateInstallerService, 
        IStatusNotifierService statusNotifierService, 
        ITmsClient tmsClient, 
        IParameterService parameterService, 
        IConfigurationService configurationService, 
        ICryptographyService cryptographyService)
    {
        _logger = logger;
        _updateInstallerService = updateInstallerService;
        _statusNotifierService = statusNotifierService;
        _tmsClient = tmsClient;
        _parameterService = parameterService;
        _configurationService = configurationService;
        _cryptographyService = cryptographyService;
    }

    /// <summary>
    /// Основной метод запуска действий.
    /// </summary>
    public async Task StartAutoUpgradeAsync()
    {
        _logger.LogInformation("Update check service started");
        
        try
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));
            do
            {
                await AuthenticationTmsClientAsync();
                await CheckAndDownloadUpdateAsync();
            }
            while (await timer.WaitForNextTickAsync());
        }
        catch (Exception e)
        {
            _logger.LogInformation(e.Message, e.InnerException);
        }
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
    
    /// <summary>
    /// Аутентификация клиента в TMS.
    /// </summary>
    private async Task AuthenticationTmsClientAsync()
    {
        if (_tmsClient.ConnectionStatus == TmsConnectionStatus.Authorized)
            return;
        
        var terminalNumber = await _parameterService.GetValueAsync(AppParameter.SerialNO111);

        if (string.IsNullOrEmpty(terminalNumber))
            return;
        
        var plainText = terminalNumber + " " + Guid.NewGuid();
        
        var password = _configurationService.CurrentSetting.TmsConfiguration!.Key;
        var salt = _configurationService.CurrentSetting.TmsConfiguration!.Salt;
        
        var workload = _cryptographyService.EncryptAes(plainText, password, Encoding.UTF8.GetBytes(salt));

        await _tmsClient.AuthenticationAsync(workload);
    }
}