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
    public Task<string> DownloadUpdatingFileAsync();

    /// <summary>
    /// Скачать и установить новую версию.
    /// </summary>
    /// <param name="packagePath">Путь к устанавливаемому файлу.</param>
    public Task InstallPackageAsync(string packagePath);
}