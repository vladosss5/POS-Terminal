using System.Security.Cryptography;
using System.Text;
using Terminal.Core.Interfaces;

namespace Terminal.Application.Services;

/// <inheritdoc/>
public class CryptographyService : ICryptographyService
{
    /// <inheritdoc/>
    public string ComputeMd5Hash(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = MD5.HashData(inputBytes);

        return Convert.ToHexString(hashBytes).ToLower();
    }

    /// <inheritdoc/>
    public bool VerifyPasswordWithMd5(string password, string hash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
            return false;
            
        var passwordHash = ComputeMd5Hash(password);
        return passwordHash.Equals(hash, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc/>
    public string EncryptAes(string plainText, string pass, byte[] salt)
    {
        using var aes = Aes.Create();
        
        var key = Rfc2898DeriveBytes.Pbkdf2(pass, salt, 10000, HashAlgorithmName.SHA256, 32);
        var iv = Rfc2898DeriveBytes.Pbkdf2(pass, salt, 10000, HashAlgorithmName.SHA256, 16);
        
        aes.Key = key;
        aes.IV = iv;

        var encryptor = aes.CreateEncryptor();

        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        
        cs.Write(plainBytes, 0, plainBytes.Length);
        cs.FlushFinalBlock();
        
        return Convert.ToBase64String(ms.ToArray());
    }
}