using System.IO;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using AndroidX.Core.Content;
using Microsoft.Extensions.Logging;
using Terminal.Application.Helpers;
using Terminal.Core.Exceptions;
using Terminal.Core.Interfaces;

namespace Terminal.Android.Services;

/// <inheritdoc/>
public class AndroidUpdateInstallerService : IUpdateInstallerService
{
    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger<AndroidUpdateInstallerService> _logger;

    /// <inheritdoc cref="ITmsService" />
    private readonly ITmsService _tmsService;
    
    /// <inheritdoc cref="Context" />
    private readonly Context _context;

    /// <summary>
    /// Каталог с загруженными пакетами обновлений.
    /// </summary>
    private readonly string _pathToLastUpdatingPatch;
    
    /// <summary>
    /// Имя файла обновления.
    /// </summary>
    private const string FileName = "terminal-updating.apk";
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public AndroidUpdateInstallerService(
        Context context, 
        ILogger<AndroidUpdateInstallerService> logger, 
        ITmsService tmsService)
    {
        _context = context;
        _logger = logger;
        _tmsService = tmsService;
        
        var folderPath = _context.FilesDir?.AbsolutePath!;
        _pathToLastUpdatingPatch = Path.Combine(folderPath, FileName);
    }

    /// <inheritdoc/>
    public void InstallUpdatingPatch()
    {
        _logger.LogInformation($"Начата установка пакета {_pathToLastUpdatingPatch}");
        
        if (string.IsNullOrEmpty(_pathToLastUpdatingPatch) || !File.Exists(_pathToLastUpdatingPatch))
        {
            _logger.LogInformation($"Не найден файл: {_pathToLastUpdatingPatch}");
            return;
        }
    
        var apkFile = new Java.IO.File(_pathToLastUpdatingPatch);
        var apkUri = FileProvider.GetUriForFile(_context, $"{_context.PackageName}.fileprovider", apkFile);
        
        var intent = new Intent(Intent.ActionInstallPackage)
            .SetDataAndType(apkUri, "application/vnd.android.package-archive")
            .SetFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.NewTask);
    
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            intent.PutExtra("android.intent.extra.NOT_UNKNOWN_SOURCE", true);
    
        _context.StartActivity(intent);
    }

    /// <inheritdoc/>
    public async Task<bool> CheckForUpdates()
    {
        var currentVersion = _context.PackageManager!.GetPackageInfo(_context.PackageName!, 0)!.VersionName;
        var newVersion = await _tmsService.GetNumberNewVersion();
        
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
        
        if (File.Exists(_pathToLastUpdatingPatch))
            File.Delete(_pathToLastUpdatingPatch);
        
        var (stream, fileHash) = await _tmsService.DownloadUpdatingFileAsync();
        
        await using var fileStream = new FileStream(_pathToLastUpdatingPatch, FileMode.Create, FileAccess.Write);
        await stream.CopyToAsync(fileStream);

        var downloadingFileHash = await HashHelper.CumputeMd5HashAsync(_pathToLastUpdatingPatch);
        if (fileHash != downloadingFileHash)
        {
            _logger.LogError($"Хеши файлов не совпали.");
            throw new InvalidFileException("Не удалось скачать файл");
        }

        await _tmsService.ConfirmReceiptUpdatingFileAsync();

        _logger.LogError($"Скачивание завершено");
    }
}