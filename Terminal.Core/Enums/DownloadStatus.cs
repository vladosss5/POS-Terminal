namespace Terminal.Core.Enums;

/// <summary>
/// Статусы скачивания.
/// </summary>
public enum DownloadStatus
{
    /// <summary>
    /// В процессе.
    /// </summary>
    InProcess,
    
    /// <summary>
    /// Прервано.
    /// </summary>
    Aborted,
    
    /// <summary>
    /// Отменено.
    /// </summary>
    Completed,
    
    /// <summary>
    /// Обновления отсутствуют.
    /// </summary>
    NotFound
}