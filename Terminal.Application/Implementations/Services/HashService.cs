using System.Security.Cryptography;
using System.Text;
using Terminal.Application.Interfaces.Services;

namespace Terminal.Application.Implementations.Services;

/// <inheritdoc/>
public class HashService : IHashService
{
    /// <inheritdoc/>
    public string ComputeMd5Hash(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        using var md5 = MD5.Create();
        
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = md5.ComputeHash(inputBytes);
            
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
}