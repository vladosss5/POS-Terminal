using System;
using System.IO;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Provider;
using AndroidX.Core.Content;
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
    /// Конструктор.
    /// </summary>
    public AndroidUpdateInstallerService(
        Context context,
        ITmsClient tmsClient)
    {
        _context = context;
        _tmsClient = tmsClient;
    }

    /// <param name="packagePath"></param>
    /// <inheritdoc/>
    public Task InstallPackageAsync(string packagePath)
    {
        try
        {
            if (!_context.PackageManager!.CanRequestPackageInstalls())
                OpenInstallUnknownAppsSettings();
        
            if (string.IsNullOrEmpty(packagePath)) return Task.CompletedTask;
        
            if (!File.Exists(packagePath)) throw new FileNotFoundException($"APK файл не найден: {packagePath}");
        
            var apkFile = new Java.IO.File(packagePath);
        
            var apkUri = FileProvider.GetUriForFile(
                _context, 
                $"{_context.PackageName}.fileprovider", 
                apkFile);

            var intent = new Intent(Intent.ActionInstallPackage);
            intent.SetDataAndType(apkUri, "application/vnd.android.package-archive");
            intent.SetFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.NewTask);
        
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                intent.PutExtra("android.intent.extra.NOT_UNKNOWN_SOURCE", true);
        
            _context.StartActivity(intent);
            
            return Task.CompletedTask;
        }
        catch (Exception exception)
        {
            return Task.FromException(exception);
        }
    }

    /// <inheritdoc/>
    public async Task<string> DownloadUpdatingFileAsync()
    {
        var (stream, fileHash) = await _tmsClient.DownloadUpdatingFileAsync();
        
        var downloadsPath = _context.GetExternalFilesDir(Environment.DirectoryDownloads)?.AbsolutePath ?? 
                            _context.FilesDir?.AbsolutePath;
        
        var apkFilePath = Path.Combine(downloadsPath!, "terminal_update.apk");
        
        if (File.Exists(apkFilePath))
            File.Delete(apkFilePath);
        
        await using var fileStream = new FileStream(apkFilePath, FileMode.Create, FileAccess.Write);
        await stream.CopyToAsync(fileStream);

        var downloadingFileHash = await HashHelper.CumputeMd5HashAsync(apkFilePath);
        if (fileHash != downloadingFileHash)
            throw new InvalidFileException("Не удалось скачать файл");
        
        return apkFilePath;
    }
    
    /// <summary>
    /// Открыть страницу настроек с разрешением на установку из неизвестных источников.
    /// </summary>
    private void OpenInstallUnknownAppsSettings()
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.O) 
            return;
        
        var intent = new Intent(Settings.ActionManageUnknownAppSources);
        intent.SetData(Uri.Parse("package:" + _context.PackageName));
        intent.SetFlags(ActivityFlags.NewTask);

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