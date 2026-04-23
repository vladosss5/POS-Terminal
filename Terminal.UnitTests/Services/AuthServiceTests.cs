using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using NUnit.Framework;
using Terminal.Application.Implementations.Services;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.DbEntities;
using Terminal.Data.Context;

namespace Terminal.UnitTests.Services;

[TestFixture]
public class AuthServiceTests
{
    private DbContextOptions<DataContext>? _dbContextOptions;
    private Mock<IHashService>? _hashServiceMock;
    private AuthService? _authService;
    private DataContext? _dbContext;

    [SetUp]
    public void SetUp()
    {
        var databaseName = Guid.NewGuid().ToString();
        
        _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
            
        _dbContext = new DataContext(_dbContextOptions);
        _hashServiceMock = new Mock<IHashService>();
        
        var dbFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        dbFactoryMock.Setup(x => x.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_dbContext);
            
        _authService = new AuthService(_hashServiceMock.Object, dbFactoryMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext?.Dispose();
    }

    [Test]
    public async Task LoginWithPasswordAsync_ValidCredentials_ReturnsTrue()
    {
        // Arrange
        const string userName = "operator_1";
        const string password = "SecurePassword123";
        const string hashedPassword = "hashed_value";
        
        _dbContext!.Users.Add(new User 
        { 
            UserId = 1, 
            Name = userName, 
            UserPassword = hashedPassword 
        });
        await _dbContext.SaveChangesAsync();
        
        _hashServiceMock!.Setup(x => x.VerifyPasswordWithMd5(password, hashedPassword))
            .Returns(true);
        
        // Act
        var result = await _authService!.LoginWithPasswordAsync(userName, password);
        
        // Assert
        Assert.That(result, Is.True);
        Assert.That(_authService.CurrentUser, Is.Not.Null);
        Assert.That(_authService.CurrentUser!.Name, Is.EqualTo(userName));
    }

    [Test]
    public async Task LoginWithPasswordAsync_UserDoesNotExist_ReturnsFalse()
    {
        // Arrange
        const string userName = "nonexistent_user";
        const string password = "any_password";
        
        // Act
        var result = await _authService!.LoginWithPasswordAsync(userName, password);
        
        // Assert
        Assert.That(result, Is.False);
        Assert.That(_authService.CurrentUser, Is.Null);
    }

    [Test]
    public async Task LoginWithCardAsync_ValidCredentials_ReturnsTrue()
    {
        // Arrange
        const string userName = "operator_2";
        const int cardNumber = 73459530;
        
        _dbContext!.Users.Add(new User 
        { 
            UserId = 2, 
            Name = userName, 
            CardNumber = cardNumber 
        });
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = await _authService!.LoginWithCardNumber(userName, cardNumber);
        
        // Assert
        Assert.That(result, Is.True);
        Assert.That(_authService.CurrentUser, Is.Not.Null);
        Assert.That(_authService.CurrentUser!.Name, Is.EqualTo(userName));
    }

    [Test]
    public async Task LogoutAsync()
    {
        // Arrange
        const string userName = "operator_2";
        const int cardNumber = 73459530;
        
        _dbContext!.Users.Add(new User 
        { 
            UserId = 2, 
            Name = userName, 
            CardNumber = cardNumber 
        });
        await _dbContext.SaveChangesAsync();
        
        await _authService!.LoginWithCardNumber(userName, cardNumber);
        
        // Act
        _authService.Logout();
        
        // Assert
        Assert.That(_authService.CurrentUser, Is.Null);
    }
}