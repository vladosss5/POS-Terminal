using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Terminal.Services.NavigationService;
using Terminal.ViewModels.Pages;

namespace Terminal.IntegrationTests.PagesTests;

[TestFixture]
public class SettingsPageViewModelTests : IntegrationTestsBase
{
    private SettingsPageViewModel? _settingsMenuPageViewModel;
    private Mock<INavigationService>? _navigationMock;

    [SetUp]
    public void InitField()
    {
        // Получаем сервисы из scope
        _settingsMenuPageViewModel = TestScope!.ServiceProvider.GetRequiredService<SettingsPageViewModel>();
        _settingsMenuPageViewModel.OnActivated(NavigationMock!.Object);
        
        _navigationMock = NavigationMock;
    }

    [Test]
    public void StepBack_WhenCalled_NavigatesToMainMenu()
    {
        // Act
        _settingsMenuPageViewModel!.StepBack();
        
        // Assert
        _navigationMock!.Verify(x => x.NavigateTo<MainMenuPageViewModel>(), Times.Once);
    }

    [Test]
    public void StepBack_CanBeCalledMultipleTimes()
    {
        // Act
        _settingsMenuPageViewModel!.StepBack();
        _settingsMenuPageViewModel!.StepBack();
        
        // Assert
        _navigationMock!.Verify(x => x.NavigateTo<MainMenuPageViewModel>(), Times.Exactly(2));
    }
}