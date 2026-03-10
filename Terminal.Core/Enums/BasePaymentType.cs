namespace Terminal.Core.Enums;

/// <summary>
/// Базовые типы оплаты.
/// </summary>
public enum BasePaymentType
{
    /// <summary>
    /// Неопределённый.
    /// </summary>
    Undefined = 0,
    
    /// <summary>
    /// Наличные.
    /// </summary>
    Cash = 1,
    
    /// <summary>
    /// Безнал.
    /// </summary>
    NonCash = 2
}