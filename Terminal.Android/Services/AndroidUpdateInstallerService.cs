using System;
using System.IO;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Provider;
using AndroidX.Core.Content;
using Microsoft.Extensions.Logging;
using Terminal.Application.Implementations.Helpers;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Exceptions;
using Terminal.Persistence.TmsClient;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;

namespace Terminal.Android.Services;

/// <inheritdoc/>
public class AndroidUpdateInstallerService : IUpdateInstallerService
{
    /// <inheritdoc cref="Context" />
    private readonly Context _context;

    /// <inheritdoc cref="ITmsClient" />
    private readonly ITmsClient _tmsClient;

    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger<AndroidUpdateInstallerService> _logger;

    /// <summary>
    /// Каталог с загруженными пакетами обновлений.
    /// </summary>
    private readonly string _pathToDownloadedPackages;

    /// <summary>
    /// Название пакета обновления.
    /// </summary>
    private const string FileName = "terminal_update.apk";

    /// <summary>
    /// Конструктор.
    /// </summary>
    public AndroidUpdateInstallerService(
        Context context,
        ITmsClient tmsClient, 
        ILogger<AndroidUpdateInstallerService> logger)
    {
        _context = context;
        _tmsClient = tmsClient;
        _logger = logger;

        _pathToDownloadedPackages = 
            _context.GetExternalFilesDir(Environment.DirectoryDownloads)?.AbsolutePath ?? 
            _context.FilesDir?.AbsolutePath!;
    }

    /// <inheritdoc/>
    public Task InstallPackageAsync()
    {
        try
        {
            var packagePath = Path.Combine(_pathToDownloadedPackages, FileName);
            
            _logger.LogInformation($"Начата установка пакета {packagePath}");
            
            if (string.IsNullOrEmpty(packagePath) || !File.Exists(packagePath))
            {
                _logger.LogInformation($"Не найден файл: {packagePath}");
                return Task.CompletedTask;
            }

            if (!_context.PackageManager!.CanRequestPackageInstalls())
                OpenInstallUnknownAppsSettings();
        
            var apkFile = new Java.IO.File(packagePath);
            var apkUri = FileProvider.GetUriForFile(_context, $"{_context.PackageName}.fileprovider", apkFile);
            
            var intent = new Intent(Intent.ActionInstallPackage)
                .SetDataAndType(apkUri, "application/vnd.android.package-archive")
                .SetFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.NewTask);
        
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                intent.PutExtra("android.intent.extra.NOT_UNKNOWN_SOURCE", true);
        
            _context.StartActivity(intent);
            
            File.Delete(packagePath);
            _logger.LogInformation($"Файл удалён: {packagePath}");
            
            return Task.CompletedTask;
        }
        catch (Exception exception)
        {
            return Task.FromException(exception);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> CheckForUpdates()
    {
        var currentVersion = _context.PackageManager!.GetPackageInfo(_context.PackageName!, 0)!.VersionName;
        var newVersion = await _tmsClient.GetNumberNewVersion();
        
        if (newVersion == null || string.IsNullOrEmpty(newVersion) || 
            currentVersion == null || string.IsNullOrEmpty(currentVersion)) 
            return false;

        var newBiggerThenCurrent = VersionHelper.RightIsBiggerThanLeft(currentVersion, newVersion);
        
        return newBiggerThenCurrent;
    }

    /// <inheritdoc/>
    public async Task DownloadUpdatingFileAsync()
    {
        _logger.LogInformation($"Начато скачивание пакета обновлений.");
        var (stream, fileHash) = await _tmsClient.DownloadUpdatingFileAsync();
        
        var apkFilePath = Path.Combine(_pathToDownloadedPackages, FileName);
        
        if (File.Exists(apkFilePath))
            File.Delete(apkFilePath);
        
        await using var fileStream = new FileStream(apkFilePath, FileMode.Create, FileAccess.Write);
        await stream.CopyToAsync(fileStream);

        var downloadingFileHash = await HashHelper.CumputeMd5HashAsync(apkFilePath);
        if (fileHash != downloadingFileHash)
        {
            _logger.LogError($"Хеши файлов не совпали.");
            throw new InvalidFileException("Не удалось скачать файл");
        }
        
        _logger.LogError($"Скачивание завершено");
    }
    
    /// <summary>
    /// Открыть страницу настроек с разрешением на установку из неизвестных источников.
    /// </summary>
    private void OpenInstallUnknownAppsSettings()
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.O) return;
        
        var intent = new Intent(Settings.ActionManageUnknownAppSources)
            .SetData(Uri.Parse("package:" + _context.PackageName))
            .SetFlags(ActivityFlags.NewTask);

        try
        {
            _context.StartActivity(intent);
        }
        catch (ActivityNotFoundException)
        {
            _context.StartActivity(new Intent(Settings.ActionSettings));
        }
    }
}