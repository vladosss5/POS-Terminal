namespace Terminal.Core.Enums;

/// <summary>
/// Типы аутентификации при открытии смены.
/// </summary>
public enum LoadMode
{
    /// <summary>
    /// Только картой.
    /// </summary>
    [FriendlyName("По карте")]
    Card,
    
    /// <summary>
    /// Только по паролю.
    /// </summary>
    [FriendlyName("По паролю")]
    Password,
    
    /// <summary>
    /// Любым из способов.
    /// </summary>
    [FriendlyName("Пароль или карта")]
    Any
}