using System.Security.Cryptography;
using System.Text;
using Microsoft.Testing.Platform.Services;
using Moq;
using NUnit.Framework;
using Terminal.Application.Interfaces.DbEntitiesServices;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.DbEntities.MainDb;
using Terminal.Core.Enums;
using Terminal.Core.Models;
using Terminal.ViewModels.Pages;

namespace Terminal.IntegrationTests.PagesTests;

[TestFixture]
public class OpenShiftPageTests : IntegrationTestsBase
{
    private OpenShiftPageViewModel? _openShiftPageViewModel;
    private IAuthService? _authService;

    private const string Password = "1432";
    private const int CardNumber = 77310317;

    private readonly User _existingOperator = new()
    {
        UserId = 1,
        Name = $"operator_1",
        CardNumber = CardNumber,
        UserPassword = Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(Password)))
    };

    [SetUp]
    public async Task InitField()
    {
        var db = await DbFactory!.CreateDbContextAsync();
        
        await db.AddAsync(_existingOperator);
        await db.SaveChangesAsync();

        TestScope!.ServiceProvider.GetRequiredService<IConfigurationService>().CurrentSetting
            .SecondsAuthenticationCanceled = 3;
        _openShiftPageViewModel = TestScope!.ServiceProvider.GetRequiredService<OpenShiftPageViewModel>();
        _openShiftPageViewModel.OnActivated(NavigationMock!.Object);
        
        _authService = TestScope!.ServiceProvider.GetRequiredService<IAuthService>();
    }
        
    [Test]
    public async Task LoginByPassword_CorrectUsernameAndPassword()
    {
        // Arrange
        var passwordButtons = Password
            .Select(symbol => _openShiftPageViewModel!.LoginButtons
                .First(x => x == symbol.ToString()))
            .ToList();

        // Act
        _openShiftPageViewModel!.SelectUser(_existingOperator);
        
        foreach (var button in passwordButtons)
            _openShiftPageViewModel.AddCharInPassword(button);

        await _openShiftPageViewModel.AuthenticationWithPasswordAsync();
        
        // Assert
        NavigationMock!.Verify(x => x.NavigateTo<MainMenuPageViewModel>(), Times.Once);
        Assert.That(_authService!.CurrentUser, Is.Not.Null);
        
        var shiftService = TestScope!.ServiceProvider.GetRequiredService<IShiftService>();
        var shift = await shiftService.GetOpenedShiftOrDefaultAsync();
        Assert.That(shift, Is.Not.Null);
    }
    
    [Test]
    public async Task LoginByPassword_IncorrectUsernameAndPassword()
    {
        // Arrange
        var passwordButtons = new[] { "1" };

        // Act
        _openShiftPageViewModel!.SelectUser(_existingOperator);
        
        foreach (var button in passwordButtons)
            _openShiftPageViewModel.AddCharInPassword(button);

        await _openShiftPageViewModel.AuthenticationWithPasswordAsync();
        
        // Assert
        NavigationMock!.Verify(x => x.NavigateTo<MainMenuPageViewModel>(), Times.Never);
        Assert.That(_authService!.CurrentUser, Is.Null);
    }
    
    [Test]
    public void LoginByCard_IncorrectCardNumber()
    {
        // Arrange
        var attempts = 0;
        const int maxAttempts = 1;

        CardReaderMock!.Setup(x => x.ReadCardAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                attempts++;
                return attempts >= maxAttempts 
                    ? CardReadResult.HardwareError("") 
                    : CardReadResult.Success(new CardInfo(
                        Convert.ToString(CardNumber + 1, 16), 
                        CardType.MifareClassic1K, 
                        []));
            });

        // Act
        _openShiftPageViewModel!.SelectUser(_existingOperator);

        // Assert
        NavigationMock!.Verify(x => x.NavigateTo<MainMenuPageViewModel>(), Times.Never);
        Assert.That(_authService!.CurrentUser, Is.Null);
    }
}