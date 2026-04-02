using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Terminal.Services.NavigationService;
using Terminal.ViewModels.Pages;

namespace Terminal.IntegrationTests.PagesTests;

[TestFixture]
public class SettingsMenuPageViewModelTests : IntegrationTestsBase
{
    private SettingsMenuPageViewModel? _settingsMenuPageViewModel;
    private Mock<INavigationService>? _navigationMock;

    [SetUp]
    public void InitField()
    {
        // Получаем сервисы из scope
        _settingsMenuPageViewModel = TestScope!.ServiceProvider.GetRequiredService<SettingsMenuPageViewModel>();
        _settingsMenuPageViewModel.OnActivated(NavigationMock!.Object);
        
        _navigationMock = NavigationMock;
    }

    [Test]
    public void Constructor_WhenInitialized_SetsTitleAndMenuItems()
    {
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(_settingsMenuPageViewModel!.Title, Is.EqualTo("Настройки"));
            Assert.That(_settingsMenuPageViewModel.MenuItems, Is.Not.Null);
            Assert.That(_settingsMenuPageViewModel.MenuItems.Count, Is.EqualTo(3));
        });
    }

    [Test]
    public void MenuItems_ContainsCorrectItems()
    {
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(_settingsMenuPageViewModel!.MenuItems[0].Title, Is.EqualTo("Типы оплаты"));
            Assert.That(_settingsMenuPageViewModel!.MenuItems[1].Title, Is.EqualTo("Аутентификация"));
            Assert.That(_settingsMenuPageViewModel!.MenuItems[2].Title, Is.EqualTo("Печать"));
        });
    }

    [Test]
    public void MenuItem_PaymentTypes_WhenExecuted_NavigatesToPaymentTypesSettings()
    {
        // Act
        _settingsMenuPageViewModel!.MenuItems[0].Command?.Execute(null);
        
        // Assert
        _navigationMock!.Verify(x => x.NavigateTo<PaymentTypesSettingsPageViewModel>(), Times.Once);
    }

    [Test]
    public void MenuItem_Authentication_WhenExecuted_NavigatesToSettingsShiftOpening()
    {
        // Act
        _settingsMenuPageViewModel!.MenuItems[1].Command?.Execute(null);
        
        // Assert
        _navigationMock!.Verify(x => x.NavigateTo<SettingsShiftOpeningPageViewModel>(), Times.Once);
    }

    [Test]
    public void MenuItem_Print_WhenExecuted_NavigatesToSettingsPrint()
    {
        // Act
        _settingsMenuPageViewModel!.MenuItems[2].Command?.Execute(null);
        
        // Assert
        _navigationMock!.Verify(x => x.NavigateTo<SettingsPrintPageViewModel>(), Times.Once);
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
    public void MenuItemCommands_AreNotExecutedWhenNavigationIsNull()
    {
        // Arrange - создаем ViewModel без навигации
        var viewModelWithoutNav = TestScope!.ServiceProvider.GetRequiredService<SettingsMenuPageViewModel>();
        
        // Act & Assert - команды не должны выбрасывать исключения при null навигации
        Assert.That(() => viewModelWithoutNav.MenuItems[0].Command?.Execute(null), Throws.Nothing);
        Assert.That(() => viewModelWithoutNav.MenuItems[1].Command?.Execute(null), Throws.Nothing);
        Assert.That(() => viewModelWithoutNav.MenuItems[2].Command?.Execute(null), Throws.Nothing);
        Assert.That(() => viewModelWithoutNav.StepBack(), Throws.Nothing);
    }

    [Test]
    public void MenuItems_AreProperlyOrdered()
    {
        // Assert
        var expectedOrder = new[] { "Типы оплаты", "Аутентификация", "Печать" };
        var actualOrder = _settingsMenuPageViewModel!.MenuItems.Select(x => x.Title).ToArray();
        
        Assert.That(actualOrder, Is.EqualTo(expectedOrder));
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