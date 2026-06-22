namespace Terminal.Application.Interfaces.Services;

/// <summary>
/// Сервис установки обновлений.
/// </summary>
public interface IUpdateInstallerService
{
    /// <summary>
    /// Загрузить файл обновления из TMS.
    /// </summary>
    /// <returns>Глобальный путь к сохранённому файлу.</returns>
    public Task DownloadUpdatingFileAsync();

    /// <summary>
    /// Скачать и установить новую версию.
    /// </summary>
    public Task InstallPackageAsync();
}