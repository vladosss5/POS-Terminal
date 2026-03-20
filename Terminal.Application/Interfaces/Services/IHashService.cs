namespace Terminal.Application.Interfaces.Services;

/// <summary>
/// Сервис хеширования.
/// </summary>
public interface IHashService
{
    /// <summary>
    /// Вычислить хеш строки с использованием MD5.
    /// </summary>
    /// <param name="input">Строка.</param>
    /// <returns>Хеш в виде строки из 32 шестнадцатеричных символов.</returns>
    public string ComputeMd5Hash(string input);

    /// <summary>
    /// Сверить пароль с хешем из БД с использованием MD5.
    /// </summary>
    /// <param name="password">Пароль.</param>
    /// <param name="hash">Сверочный хеш.</param>
    /// <returns>True - если совпало.</returns>
    public bool VerifyPasswordWithMd5(string password, string hash);
}