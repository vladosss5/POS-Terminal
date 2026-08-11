using Terminal.Core.Enums;

namespace Terminal.Core.Entities.Models;

/// <summary>
/// Dto всплывающего уведомления.
/// </summary>
public sealed record Popup
{
    /// <summary>
    /// Текст сообщения.
    /// </summary>
    public string Message { get; } = null!;
    
    /// <summary>
    /// Тип всплывающего уведомления.
    /// </summary>
    public PopupType Type { get; }
    
    /// <summary>
    /// Продолжительность показа в мс.
    /// </summary>
    public int DurationMs { get; } 
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="message">Текст сообщения.</param>
    /// <param name="type">Тип оповещения.</param>
    /// <param name="durationMs">Время показа в мс.</param>
    /// <exception cref="ArgumentException">Ошибка, если message является null или пустой строкой.</exception>
    public Popup(string message, PopupType type, int durationMs = 5000)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        
        Message = message;
        Type = type;
        DurationMs = durationMs;
    }
}