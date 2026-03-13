namespace Terminal.Core.Enums;

/// <summary>
/// Типы ошибок считывателя карт
/// </summary>
public enum CardReaderErrorType
{
    /// <summary>
    /// Тайм-аут ожидания карты
    /// </summary>
    Timeout,
    
    /// <summary>
    /// Операция отменена пользователем
    /// </summary>
    Cancelled,
    
    /// <summary>
    /// Ошибка оборудования
    /// </summary>
    Hardware,
    
    /// <summary>
    /// Ошибка сервиса Sunyard
    /// </summary>
    Service,
    
    /// <summary>
    /// Ошибка подключения к сервису
    /// </summary>
    Connection
}