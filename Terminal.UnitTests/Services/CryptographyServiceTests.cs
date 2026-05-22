using NUnit.Framework;
using Terminal.Application.Implementations.Services;

namespace Terminal.UnitTests.Services;

[TestFixture]
public class CryptographyServiceTests
{
    private CryptographyService? _hashService;

    [SetUp]
    public void SetUp()
    {
        _hashService = new CryptographyService();
    }

    [TearDown]
    public void TearDown()
    { }

    [Test]
    public void ComputeMd5Hash_ValidData_ReturnHash()
    {
        // Arrange
        const string input = "QWEasd123!@#";
        const string verifyHash = "34d979f92a760a9e0ba9037654d6f874";
        
        // Act
        var resultHas = _hashService!.ComputeMd5Hash(input);
        
        // Assert
        Assert.AreEqual(resultHas, verifyHash);
    }
    
    [Test]
    public void ComputeMd5Hash_InvalidData_ReturnHash()
    {
        // Arrange
        const string input = "QWEasd123!@#";
        const string verifyHash = "34d979f92a760a9e0ab9037654d6f874";
        
        // Act
        var resultHas = _hashService!.ComputeMd5Hash(input);
        
        // Assert
        Assert.AreNotEqual(resultHas, verifyHash);
    }
    
    [Test]
    public void VerifyPasswordWithMd5_ValidPasswordAndHash_ReturnsTrue()
    {
        // Arrange
        var password = "password123";
        var hash = _hashService.ComputeMd5Hash(password);
            
        // Act
        var result = _hashService.VerifyPasswordWithMd5(password, hash);
            
        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void VerifyPasswordWithMd5_InvalidPassword_ReturnsFalse()
    {
        // Arrange
        var password = "wrong_password";
        var hash = _hashService.ComputeMd5Hash("correct_password");
            
        // Act
        var result = _hashService.VerifyPasswordWithMd5(password, hash);
            
        // Assert
        Assert.That(result, Is.False);
    }
}