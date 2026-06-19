using System.IO;
using System.Threading.Tasks;
using Android.Content;
using AndroidX.Core.Content;
using Terminal.Application.Interfaces.Services;
using Environment = Android.OS.Environment;

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
        var downloadsPath = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDownloads)!.AbsolutePath;
        var apkFileName = "terminal_update.apk";
        var apkFilePath = Path.Combine(downloadsPath, apkFileName);
        var apkFile = new Java.IO.File(apkFilePath);
        var apkUri = FileProvider.GetUriForFile(_context, $"{_context.PackageName}.fileprovider", apkFile);
        
        var intent = new Intent(Intent.ActionInstallPackage);
        intent.SetDataAndType(apkUri, "application/vnd.android.package-archive");
        intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.GrantReadUriPermission);
        _context.StartActivity(intent);
    }
}