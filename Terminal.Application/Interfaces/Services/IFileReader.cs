namespace Terminal.Application.Interfaces.Services;

/// <summary>
/// Сервис чтения файлов.
/// </summary>
public interface IFileReader
{
    /// <summary>
    /// Вычитка содержимого файла.
    /// </summary>
    /// <param name="path">Путь к файлу.</param>
    /// <returns>Текстовое содержимое.</returns>
    public Task<string> ReadAllTextAsync(string path);
    
    /// <summary>
    /// Получить файлы из директории.
    /// </summary>
    /// <param name="directoryPath">Путь до директории.</param>
    /// <param name="searchPattern"></param>
    /// <returns></returns>
    public Task<IEnumerable<string>> GetFilesAsync(string directoryPath, string searchPattern);
    
    /// <summary>
    /// Проверка существования файла.
    /// </summary>
    /// <param name="path">Путь до файла.</param>
    /// <returns>Существует ли.</returns>
    public bool FileExists(string path);
}