namespace Terminal.Core.Models;

/// <summary>
/// Типы файлов для передачи
/// </summary>
public static class FileTypes
{
    /// <summary>Конфигурация</summary>
    public const short Configuration = 0;
    
    /// <summary>Обновление ПО</summary>
    public const short SoftwareUpdate = 1;
    
    /// <summary>Справочные данные</summary>
    public const short ReferenceData = 2;
    
    /// <summary>Журнал событий</summary>
    public const short EventLog = 3;
    
    /// <summary>Отчёт</summary>
    public const short Report = 4;
}