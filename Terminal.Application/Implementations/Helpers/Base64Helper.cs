using System.Text;

namespace Terminal.Application.Implementations.Helpers;

public static class Base64Helper
{
    /// <summary>
    /// Декодирует Base64 строку в обычную строку.
    /// </summary>
    /// <param name="base64String">Base64 строка/</param>
    /// <returns>Декодированная строка или null при ошибке/</returns>
    public static string? DecodeFromBase64(string? base64String)
    {
        if (string.IsNullOrWhiteSpace(base64String))
            return null;
        
        try
        {
            var decodedBytes = Convert.FromBase64String(base64String);
            return Encoding.UTF8.GetString(decodedBytes);
        }
        catch (FormatException)
        {
            return null;
        }
    }
    
    /// <summary>
    /// Кодирует строку в Base64.
    /// </summary>
    /// <param name="plainText">Обычная строка</param>
    /// <returns>Base64 строка или null при ошибке</returns>
    public static string? EncodeToBase64(string? plainText)
    {
        if (string.IsNullOrWhiteSpace(plainText))
            return null;
        
        var bytesToEncode = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(bytesToEncode);
    }
    
    /// <summary>
    /// Декодирует Base64 строку с указанной кодировкой.
    /// </summary>
    public static string? DecodeFromBase64(string? base64String, Encoding encoding)
    {
        if (string.IsNullOrWhiteSpace(base64String))
            return null;
        
        try
        {
            var decodedBytes = Convert.FromBase64String(base64String);
            return encoding.GetString(decodedBytes);
        }
        catch (FormatException)
        {
            return null;
        }
    }
    
    /// <summary>
    /// Проверяет, является ли строка корректным Base64.
    /// </summary>
    public static bool IsValidBase64(string? base64String)
    {
        if (string.IsNullOrWhiteSpace(base64String))
            return false;
        
        try
        {
            Convert.FromBase64String(base64String);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}