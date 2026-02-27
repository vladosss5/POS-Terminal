namespace Terminal.Application.Interfaces.Services;

/// <summary>
/// Сервис работы с каталогами и файлами.
/// </summary>
public interface IFileExplorer
{
    /// <summary>
    /// Копировать директорию с БД в директорию загрузок.
    /// </summary>
    /// <remarks>
    /// В основном используется для проверки БД на Android платформе.
    /// </remarks>
    public Task CopyDataBaseDirectoryToDownloadsAsync();
}