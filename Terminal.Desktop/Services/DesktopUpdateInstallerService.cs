using System;
using System.Threading.Tasks;
using Terminal.Application.Interfaces.Services;

namespace Terminal.Desktop.Services;

/// <inheritdoc/>
public class DesktopUpdateInstallerService : IUpdateInstallerService
{
    /// <inheritdoc/>
    public async Task DownloadUpdatingFileAsync()
    {
        throw new NotImplementedException("Обновления настольной версии не реализованы.");
    }

    /// <inheritdoc/>
    public async Task InstallPackageAsync()
    {
        throw new NotImplementedException("Обновления настольной версии не реализованы.");
    }
}