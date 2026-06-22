using System.Threading.Tasks;
using Terminal.Application.Interfaces.Services;

namespace Terminal.Desktop.Services;

/// <inheritdoc/>
public class DesktopUpdateInstallerService : IUpdateInstallerService
{
    public async Task DownloadUpdatingFileAsync()
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task InstallPackageAsync()
    {
        throw new System.NotImplementedException();
    }
}