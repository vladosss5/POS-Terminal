using System;
using System.Threading.Tasks;
using Terminal.Core.Interfaces;

namespace Terminal.Desktop.Services;

/// <inheritdoc/>
public class DesktopUpdateInstallerService : IUpdateInstallerService
{
    public async Task<bool> CheckForUpdates()
    {
        return false;
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task DownloadUpdatingFileAsync()
    {
        return;
        throw new NotImplementedException("Обновления настольной версии не реализованы.");
    }

    /// <inheritdoc/>
    public async Task InstallUpdatingPatchAsync()
    {
        return;
        throw new NotImplementedException("Обновления настольной версии не реализованы.");
    }
}