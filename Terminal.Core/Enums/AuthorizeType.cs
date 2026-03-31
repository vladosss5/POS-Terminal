namespace Terminal.Core.Enums;

/// <summary>
/// Типы аутентификации при открытии смены.
/// </summary>
public enum AuthorizeType
{
    /// <summary>
    /// Любым из способов.
    /// </summary>
    [FriendlyName("Пароль или карта")]
    Any = 0,
    
    /// <summary>
    /// Только по паролю.
    /// </summary>
    [FriendlyName("По паролю")]
    Password = 1,
    
    /// <summary>
    /// Только картой.
    /// </summary>
    [FriendlyName("По карте")]
    MifareCard = 2
}