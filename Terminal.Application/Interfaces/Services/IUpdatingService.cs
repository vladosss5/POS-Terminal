namespace Terminal.Application.Interfaces.Services;

/// <summary>
/// Сервис для обновления приложений
/// </summary>
public interface IUpdatingService
{
    /// <summary>
    /// Проверить новые версии программы.
    /// </summary>
    /// <returns></returns>
    public Task<bool> CheckNewVersionAsync();

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