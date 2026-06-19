using System.IO;
using System.Threading.Tasks;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Provider;
using AndroidX.Core.Content;
using Terminal.Application.Interfaces.Services;

namespace Terminal.Android.Services;

/// <inheritdoc/>
public class AndroidUpdateInstallerService : IUpdateInstallerService
{
    private readonly Context _context;

    public AndroidUpdateInstallerService(Context context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task InstallDownloadedVersionAsync()
    {
        if (!_context.PackageManager!.CanRequestPackageInstalls())
            OpenInstallUnknownAppsSettings();
            
        var downloadsPath = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDownloads)!.AbsolutePath;
        var apkFilePath = Path.Combine(downloadsPath, "terminal_update.apk");
        var apkFile = new Java.IO.File(apkFilePath);
        var apkUri = FileProvider.GetUriForFile(_context, $"{_context.PackageName}.fileprovider", apkFile);
        
        var intent = new Intent(Intent.ActionInstallPackage);
        intent.SetData(apkUri);
        intent.SetFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.NewTask);
        _context.StartActivity(intent);
    }
    
    private void OpenInstallUnknownAppsSettings()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O) 
        {
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
}