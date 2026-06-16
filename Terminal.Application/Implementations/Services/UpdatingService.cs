using Terminal.Application.Interfaces.Services;

namespace Terminal.Application.Implementations.Services;

/// <inheritdoc/>
public class UpdatingService : IUpdatingService
{
    /// <inheritdoc cref="IUpdateInstallerService" />
    private readonly IUpdateInstallerService _installerService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public UpdatingService(IUpdateInstallerService installerService)
    {
        _installerService = installerService;
    }

    /// <inheritdoc/>
    public async Task<bool> CheckNewVersionAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task DownloadUpdateAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task InstallDownloadedVersionAsync() => await _installerService.InstallDownloadedVersionAsync();
}