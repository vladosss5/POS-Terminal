using System.Threading.Tasks;
using Terminal.Application.Interfaces.Services;

namespace Terminal.Android.Services;

/// <inheritdoc/>
public class AndroidUpdateInstallerService : IUpdateInstallerService
{
    /// <inheritdoc/>
    public async Task InstallDownloadedVersionAsync()
    {
        throw new System.NotImplementedException();
    }
}