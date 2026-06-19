namespace Terminal.Application.Interfaces.Services;

/// <summary>
/// Сервис для обновления приложений
/// </summary>
public interface IUpdatingService
{
    /// <summary>
    /// Скачать обновление.
    /// </summary>
    /// <returns></returns>
    public Task DownloadUpdateAsync();

    /// <summary>
    /// Установить скачанное обновление.
    /// </summary>
    /// <returns></returns>
    public Task InstallDownloadedVersionAsync();
}