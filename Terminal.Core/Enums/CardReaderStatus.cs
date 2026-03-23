namespace Terminal.Core.Enums;

/// <summary>
/// Статусы считывателя карт.
/// </summary>
public enum CardReaderStatus
{
    /// <summary>
    /// В процессе подключения.
    /// </summary>
    Connecting,
    
    /// <summary>
    /// Ожидается поднесение карты.
    /// </summary>
    WaitingCard,
    
    /// <summary>
    /// Успешно считано.
    /// </summary>
    SuccessfullyRead,
    
    /// <summary>
    /// Ошибка чтения.
    /// </summary>
    ErrorRead,
    
    /// <summary>
    /// Операция отменена.
    /// </summary>
    OperationCancelled,
    
    /// <summary>
    /// Внутреннее исключение.
    /// </summary>
    InternalError
}