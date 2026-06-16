namespace Terminal.Application.Interfaces.Services;

/// <summary>
/// Сервис установки обновлений.
/// </summary>
public interface IUpdateInstallerService
{
    /// <summary>
    /// Установить скачанную версию.
    /// </summary>
    public Task InstallDownloadedVersionAsync();
}