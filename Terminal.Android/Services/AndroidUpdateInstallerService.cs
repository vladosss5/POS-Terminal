using System.IO;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using AndroidX.Core.Content;
using Microsoft.Extensions.Logging;
using Terminal.Application.Helpers;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Enums;
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

    private readonly IParameterService _parameterService;

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
        ITmsService tmsService, IParameterService parameterService)
    {
        _context = context;
        _logger = logger;
        _tmsService = tmsService;
        _parameterService = parameterService;

        var folderPath = _context.FilesDir?.AbsolutePath!;
        _pathToLastUpdatingPatch = Path.Combine(folderPath, FileName);
    }

    /// <inheritdoc/>
    public async Task InstallUpdatingPatchAsync()
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
        
        var fileHash = await HashHelper.CumputeMd5HashAsync(_pathToLastUpdatingPatch);
        await _parameterService.SetValueAsync(AppParameter.HashLastInstalledPatch, fileHash);
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

        var downloadingFileHash = "";
        
        if (File.Exists(_pathToLastUpdatingPatch))
        {
            downloadingFileHash = await HashHelper.CumputeMd5HashAsync(_pathToLastUpdatingPatch);
            var hashLastInstalledPatch = await _parameterService.GetValueAsync(AppParameter.HashLastInstalledPatch);

            if (downloadingFileHash != hashLastInstalledPatch)
                return;
            
            File.Delete(_pathToLastUpdatingPatch);
        }
        
        var (stream, fileHash) = await _tmsService.DownloadUpdatingFileAsync();
        
        await using var fileStream = new FileStream(_pathToLastUpdatingPatch, FileMode.Create, FileAccess.Write);
        await stream.CopyToAsync(fileStream);
        
        if (fileHash != downloadingFileHash)
        {
            _logger.LogError($"Хеши файлов не совпали.");
            throw new InvalidFileException("Не удалось скачать файл");
        }

        await _tmsService.ConfirmReceiptUpdatingFileAsync();

        _logger.LogError($"Скачивание завершено");
    }
}