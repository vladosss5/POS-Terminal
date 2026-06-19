using Terminal.Application.Interfaces.Services;
using Terminal.Core.Enums;
using Terminal.Persistence.TmsClient;

namespace Terminal.Application.Implementations.Services;

/// <inheritdoc/>
public class UpdatingService : IUpdatingService
{
    /// <inheritdoc cref="IUpdateInstallerService" />
    private readonly IUpdateInstallerService _installerService;

    /// <inheritdoc cref="ITmsClient" />
    private readonly ITmsClient _tmsClient;

    /// <inheritdoc cref="IParameterService" />
    private readonly IParameterService _parameterService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public UpdatingService(
        IUpdateInstallerService installerService, 
        ITmsClient tmsClient, 
        IParameterService parameterService)
    {
        _installerService = installerService;
        _tmsClient = tmsClient;
        _parameterService = parameterService;
    }
    

    /// <inheritdoc/>
    public async Task DownloadUpdateAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task InstallDownloadedVersionAsync() => await _installerService.InstallDownloadedVersionAsync();
}