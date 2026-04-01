using System.Security.Cryptography;
using System.Text;
using Microsoft.Testing.Platform.Services;
using Moq;
using NUnit.Framework;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.DbEntities;
using Terminal.Core.Enums;
using Terminal.Core.Models;
using Terminal.ViewModels.Pages;

namespace Terminal.IntegrationTests.PagesTests;

[TestFixture]
public class OpenShiftPageTests : IntegrationTestsBase
{
    private OpenShiftPageViewModel? _openShiftPageViewModel;

    private const string Password = "1432";

    private readonly User _existingOperator = new()
    {
        UserId = 1,
        Name = $"operator_1",
        UserPassword = Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(Password)))
    };

    [OneTimeSetUp]
    public async Task InitField()
    {
        var db = await DbFactory!.CreateDbContextAsync();
        
        await db.AddAsync(_existingOperator);
        await db.SaveChangesAsync();
        
        _openShiftPageViewModel = Services!.GetRequiredService<OpenShiftPageViewModel>();
        _openShiftPageViewModel.OnActivated(NavigationMock!.Object);
    }
        
    [Test]
    public async Task LoginByPassword_CorrectUsernameAndPassword()
    {
        // Arrange
        var passwordButtons = Password
            .Select(symbol => _openShiftPageViewModel!.LoginButtons
                .First(x => x.Content == symbol.ToString()))
            .ToList();

        passwordButtons.Add(new LoginButton
        {
            Content = "enter.png", 
            ContentIsImage = true, 
            Type = LoginButtonTypes.Enter
        });
        

        // Act
        _openShiftPageViewModel!.SelectUser(_existingOperator);
        
        foreach (var button in passwordButtons)
            await _openShiftPageViewModel.ButtonClick(button);
        
        
        // Assert
        NavigationMock!.Verify(x => x.NavigateTo<MainMenuPageViewModel>(), Times.Once);
        var authService = Services!.GetRequiredService<IAuthService>();
        Assert.That(authService.CurrentUser, Is.Not.Null);
    }
    
    [Test]
    public async Task LoginByPassword_IncorrectUsernameAndPassword()
    {
        // Arrange
        var passwordButtons = new LoginButton[]
        {
            new()
            {
                Content = "1",
                ContentIsImage = false,
                Type = LoginButtonTypes.Digit
            },
            new()
            {
                Content = "enter.png",
                ContentIsImage = true,
                Type = LoginButtonTypes.Enter
            }
        };
        

        // Act
        _openShiftPageViewModel!.SelectUser(_existingOperator);
        
        foreach (var button in passwordButtons)
            await _openShiftPageViewModel.ButtonClick(button);
        
        
        // Assert
        NavigationMock!.Verify(x => x.NavigateTo<MainMenuPageViewModel>(), Times.Never);
        var authService = Services!.GetRequiredService<IAuthService>();
        Assert.That(authService.CurrentUser, Is.Null);
    }
}