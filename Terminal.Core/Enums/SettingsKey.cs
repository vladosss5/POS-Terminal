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
    Bill = 3
}