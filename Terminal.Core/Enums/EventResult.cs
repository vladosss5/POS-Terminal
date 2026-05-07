// Terminal.Core/Enums/EventResult.cs
namespace Terminal.Core.Enums;

/// <summary>
/// Результат события
/// </summary>
public enum EventResult
{
    /// <summary>
    /// Готов к отправке
    /// </summary>
    ReadyToSend = 0,
    
    /// <summary>
    /// Ошибка при подготовке к отправке
    /// </summary>
    ReadyToSendError = 1,
    
    /// <summary>
    /// Отправлено успешно
    /// </summary>
    Sent = 2,
    
    /// <summary>
    /// Общий результат (информационное сообщение)
    /// </summary>
    CommonResult = 3
}