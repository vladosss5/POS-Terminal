using System.Security.Cryptography;

namespace Terminal.Application.Helpers;

/// <summary>
/// Статический хелпер хеширования.
/// </summary>
public static class HashHelper
{
    /// <summary>
    /// Вычислить MD5 хеш из файла.
    /// </summary>
    /// <param name="filePath">Глобальный путь к файлу.</param>
    /// <returns>Хеш-строка.</returns>
    public static async Task<string> CumputeMd5HashAsync(string filePath)
    {
        await using var stream = File.OpenRead(filePath);
        using var md5 = MD5.Create();
        var hash = await md5.ComputeHashAsync(stream);
        
        return Convert.ToHexString(hash);
    }
}