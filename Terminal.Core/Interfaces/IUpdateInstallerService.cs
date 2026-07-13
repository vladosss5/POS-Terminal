namespace Terminal.Core.Interfaces;

/// <summary>
/// Сервис установки обновлений.
/// </summary>
public interface IUpdateInstallerService
{
    /// <summary>
    /// Загрузить файл обновления из TMS.
    /// </summary>
    public Task DownloadUpdatingFileAsync();

    /// <summary>
    /// Установить скачанный пакет обновлений.
    /// </summary>
    public Task InstallPackageAsync();
}