namespace Terminal.Core.Interfaces;

/// <summary>
/// Сервис криптографии.
/// </summary>
public interface ICryptographyService
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

    /// <summary>
    /// Зашифровать текст с помощью AES.
    /// </summary>
    /// <param name="plainText">Текст.</param>
    /// <param name="pass">Ключ.</param>
    /// <param name="salt">Соль.</param>
    /// <returns>Зашифрованное сообщение.</returns>
    public string EncryptAes(string plainText, string pass, byte[] salt);
}