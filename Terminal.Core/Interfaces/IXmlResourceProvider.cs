namespace Terminal.Core.Interfaces;

/// <summary>
/// Предоставляет платформо-независимый доступ к XML-ресурсам приложения
/// </summary>
public interface IXmlResourceProvider
{
    /// <summary>
    /// Асинхронно загружает содержимое XML-файла по имени
    /// </summary>
    /// <param name="fileName">Имя файла без пути (например, "param.xml")</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Строковое содержимое XML-файла</returns>
    Task<string> LoadXmlContentAsync(string fileName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Проверяет существование XML-файла
    /// </summary>
    Task<bool> XmlFileExistsAsync(string fileName, CancellationToken cancellationToken = default);
}