namespace Terminal.Core.Interfaces;

/// <summary>
/// Сервис установки обновлений.
/// </summary>
public interface IUpdateInstallerService
{
    /// <summary>
    /// Проверить обновления на TMS.
    /// </summary>
    /// <returns>True, если версия на TMS выше чем текущая.</returns>
    public Task<bool> CheckForUpdates();
    
    /// <summary>
    /// Загрузить файл обновления из TMS.
    /// </summary>
    public Task DownloadUpdatingFileAsync();

    /// <summary>
    /// Установить скачанный пакет обновлений.
    /// </summary>
    public Task InstallUpdatingPatchAsync();
}