namespace Terminal.Core.Enums;

/// <summary>
/// Номера настроек терминала.
/// </summary>
public enum SettingsKey
{
    /// <summary>
    /// Последний номер смены.
    /// </summary>
    [FriendlyName("Последний номер смены")]
    Shift = 1,
    
    /// <summary>
    /// Последний номер чека.
    /// </summary>
    [FriendlyName("Последний номер чека")]
    Sale = 2,
    
    /// <summary>
    /// Последний номер корзины.
    /// </summary>
    [FriendlyName("Последний номер корзины")]
    Bill = 3,
    
    /// <summary>
    /// Включёна ли оплата наличными.
    /// </summary>
    [FriendlyName("Наличные")]
    CashPaymentTypeEnabled = 4,
    
    /// <summary>
    /// Включёна ли оплата топливными картами.
    /// </summary>
    [FriendlyName("Топливная карта")]
    FuelCardPaymentTypeEnabled = 5,
    
    /// <summary>
    /// Включёна ли оплата ведомостями.
    /// </summary>
    [FriendlyName("Ведомость")]
    FuelStatementPaymentTypeEnabled = 6,
    
    /// <summary>
    /// Включёна ли оплата талонами.
    /// </summary>
    [FriendlyName("Талон")]
    FuelTalonPaymentTypeEnabled = 7,
    
    /// <summary>
    /// Включёна ли оплата банковскими картами.
    /// </summary>
    [FriendlyName("Банковская карта")]
    BankCardPaymentTypeEnabled = 8,
}