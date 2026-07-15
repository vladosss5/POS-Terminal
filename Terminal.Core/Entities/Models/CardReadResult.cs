using Terminal.Core.Enums;

namespace Terminal.Core.Entities.Models;

/// <summary>
/// Результат операции считывания карты (Result Pattern).
/// </summary>
public readonly record struct CardReadResult
{
    /// <summary>
    /// Успешность операции.
    /// </summary>
    public bool IsSuccess { get; }
    
    /// <summary>
    /// Информация о карте (если успешно).
    /// </summary>
    public CardInfo? Card { get; }
    
    /// <summary>
    /// Сообщение об ошибке.
    /// </summary>
    public string? ErrorMessage { get; }
    
    /// <summary>
    /// Тип ошибки.
    /// </summary>
    public CardReaderErrorType? ErrorType { get; }

    /// <summary>
    /// Конструктор.
    /// </summary>
    private CardReadResult(CardInfo card)
    {
        IsSuccess = true;
        Card = card;
        ErrorMessage = null;
        ErrorType = null;
    }

    /// <summary>
    /// Перегрузка конструктора.
    /// </summary>
    private CardReadResult(string errorMessage, CardReaderErrorType errorType)
    {
        IsSuccess = false;
        Card = null;
        ErrorMessage = errorMessage;
        ErrorType = errorType;
    }

    /// <summary>
    /// Создать успешный результат.
    /// </summary>
    public static CardReadResult Success(CardInfo card) => new(card);
    
    /// <summary>
    /// Тайм-аут ожидания.
    /// </summary>
    public static CardReadResult Timeout() => new("Timeout waiting for card", CardReaderErrorType.Timeout);
    
    /// <summary>
    /// Отмена операции.
    /// </summary>
    public static CardReadResult Cancelled() => new("Operation cancelled", CardReaderErrorType.Cancelled);
    
    /// <summary>
    /// Ошибка оборудования.
    /// </summary>
    public static CardReadResult HardwareError(string message) => new(message, CardReaderErrorType.Hardware);
    
    /// <summary>
    /// Ошибка сервиса.
    /// </summary>
    public static CardReadResult ServiceError(string message) => new(message, CardReaderErrorType.Service);
    
    /// <summary>
    /// Ошибка подключения.
    /// </summary>
    public static CardReadResult ConnectionError(string message) => new(message, CardReaderErrorType.Connection);
}