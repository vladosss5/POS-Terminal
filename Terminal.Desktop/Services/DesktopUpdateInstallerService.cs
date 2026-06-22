using System.Threading.Tasks;
using Terminal.Application.Interfaces.Services;

namespace Terminal.Desktop.Services;

/// <inheritdoc/>
public class DesktopUpdateInstallerService : IUpdateInstallerService
{
    public async Task<string> DownloadUpdatingFileAsync()
    {
        throw new System.NotImplementedException();
    }

    /// <param name="packagePath"></param>
    /// <inheritdoc/>
    public async Task InstallPackageAsync(string packagePath)
    {
        throw new System.NotImplementedException();
    }
}