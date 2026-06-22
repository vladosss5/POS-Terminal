namespace Terminal.Application.Interfaces.Services;

/// <summary>
/// Сервис установки обновлений.
/// </summary>
public interface IUpdateInstallerService
{
    /// <summary>
    /// Скачать и установить новую версию.
    /// </summary>
    public Task InstallUpdatePackageAsync();
}