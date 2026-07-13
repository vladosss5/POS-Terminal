using Terminal.Core.Entities.DbEntities.MainDb;

namespace Terminal.Application.Interfaces.Services;

/// <summary>
/// Сервис авторизации.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Авторизованный сейчас пользователь.
    /// </summary>
    public User? CurrentUser { get; }

    /// <summary>
    /// Выполнить вход по паролю.
    /// </summary>
    /// <param name="userName">Имя пользователя.</param>
    /// <param name="password">Пароль.</param>
    /// <returns>True если вход успешен.</returns>
    public Task<bool> LoginWithPasswordAsync(string userName, string password);

    /// <summary>
    /// Выполнить вход по номеру карты.
    /// </summary>
    /// <param name="userName">Имя пользователя.</param>
    /// <param name="cardNumber">Номер карты.</param>
    /// <returns>True если вход успешен.</returns>
    public Task<bool> LoginWithCardNumber(string userName, int cardNumber);

    /// <summary>
    /// Аутентифицировать текущего оператора повторно.
    /// </summary>
    /// <param name="password">Пароль для проверки.</param>
    /// <returns>True когда хэш пароля текущего оператора совпадает с хешем введённого.</returns>
    public bool AuthenticateOperator(string password);
    
    /// <summary>
    /// Выполнить выход.
    /// </summary>
    public void Logout();
}