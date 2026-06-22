using System.IO;
using System.Threading.Tasks;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Provider;
using AndroidX.Core.Content;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Enums;
using Terminal.Persistence.TmsClient;

namespace Terminal.Android.Services;

/// <inheritdoc/>
public class AndroidUpdateInstallerService : IUpdateInstallerService
{
    /// <inheritdoc cref="Context" />
    private readonly Context _context;
    
    /// <inheritdoc cref="IParameterService" />
    private readonly IParameterService _parameterService;

    /// <inheritdoc cref="ITmsClient" />
    private readonly ITmsClient _tmsClient;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public AndroidUpdateInstallerService(
        Context context, 
        IParameterService parameterService, 
        ITmsClient tmsClient)
    {
        _context = context;
        _parameterService = parameterService;
        _tmsClient = tmsClient;
    }

    /// <inheritdoc/>
    public async Task InstallDownloadedVersionAsync()
    {
        if (!_context.PackageManager!.CanRequestPackageInstalls())
            OpenInstallUnknownAppsSettings();

        var filePath = await DownloadUpdatingFileAsync();

        InstallApkAsync(filePath);
    }

    /// <summary>
    /// Загрузить обновление из TMS.
    /// </summary>
    /// <returns>Путь к скачанному файлу.</returns>
    private async Task<string> DownloadUpdatingFileAsync()
    {
        await using var stream = await _tmsClient.DownloadUpdatingFileAsync();
        
        var downloadsPath = _context.GetExternalFilesDir(Environment.DirectoryDownloads)?.AbsolutePath ?? 
                            _context.FilesDir?.AbsolutePath;
        
        var apkFilePath = Path.Combine(downloadsPath!, "terminal_update.apk");
        
        if (File.Exists(apkFilePath))
            File.Delete(apkFilePath);
        
        await using var fileStream = new FileStream(apkFilePath, FileMode.Create, FileAccess.Write);
        await stream.CopyToAsync(fileStream);
        
        return apkFilePath;
    }

    /// <summary>
    /// Устанавливает APK файл.
    /// </summary>
    private void InstallApkAsync(string apkFilePath)
    {
        if (!File.Exists(apkFilePath))
            throw new FileNotFoundException($"APK файл не найден: {apkFilePath}");

        var apkFile = new Java.IO.File(apkFilePath);
            
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