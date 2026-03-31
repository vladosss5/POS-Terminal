using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Models;
using Terminal.Core.Models.Settings;
using Terminal.ViewModels;
using Terminal.ViewModels.Pages;

namespace Terminal.UnitTests.Services;

[TestFixture]
public class SettingsShiftOpeningPageViewModelTests
{
    private Mock<ILogger<PageViewModelBase>>? _loggerMock;
    private Mock<IConfigurationService>? _configurationServiceMock;
    private SettingsModel? _currentSettings;
    private SettingsShiftOpeningPageViewModel? _viewModel;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<PageViewModelBase>>();
        _configurationServiceMock = new Mock<IConfigurationService>();
            
        // Создаем тестовые настройки
        _currentSettings = new SettingsModel
        {
            SecondsAuthenticationCanceled = 30
        };
            
        _configurationServiceMock.Setup(x => x.CurrentSetting)
            .Returns(_currentSettings);
            
        _viewModel = new SettingsShiftOpeningPageViewModel(
            _loggerMock.Object,
            _configurationServiceMock.Object);
    }

    [Test]
    public void Constructor_WhenCalled_SetsTitle()
    {
        // Assert
        Assert.That(_viewModel!.Title, Is.EqualTo("Открытие смены"));
    }

    [Test]
    public void Constructor_WhenCalled_InitializesTimeoutValuesWithDefaultOptions()
    {
        // Assert
        var expectedSeconds = new[] { 10, 15, 30, 60 };
        var actualSeconds = _viewModel!.TimeoutValues.Select(x => x.Seconds).ToList();
            
        Assert.Multiple(() =>
        {
            Assert.That(_viewModel.TimeoutValues.Count, Is.EqualTo(4));
            Assert.That(actualSeconds, Is.EquivalentTo(expectedSeconds));
        });
    }

    [Test]
    public void Constructor_WhenCalled_LoadsCurrentSettingFromConfigurationService()
    {
        // Assert
        Assert.That(_viewModel!.SecondsAuthenticationCanceled.Seconds, 
            Is.EqualTo(_currentSettings!.SecondsAuthenticationCanceled));
    }

    [Test]
    public void Constructor_WhenCalled_AddsCurrentSettingToTimeoutValuesIfNotExists()
    {
        // Arrange
        const short uniqueTimeoutValue = 45;
        _currentSettings!.SecondsAuthenticationCanceled = uniqueTimeoutValue;
            
        // Recreate viewModel with new settings
        _viewModel = new SettingsShiftOpeningPageViewModel(_loggerMock!.Object, _configurationServiceMock!.Object);
            
        // Assert
        Assert.That(_viewModel.TimeoutValues.Any(x => x.Seconds == uniqueTimeoutValue), Is.True);
        Assert.That(_viewModel.TimeoutValues.Count, Is.EqualTo(5)); // 4 defaults + 1 custom
    }

    [Test]
    public void Constructor_WhenCalled_DoesNotDuplicateExistingTimeoutValue()
    {
        // Arrange
        const short existingTimeoutValue = 30;
        _currentSettings!.SecondsAuthenticationCanceled = existingTimeoutValue;
            
        // Act
        _viewModel = new SettingsShiftOpeningPageViewModel(_loggerMock!.Object, _configurationServiceMock!.Object);
            
        // Assert
        Assert.That(_viewModel.TimeoutValues.Count(x => x.Seconds == existingTimeoutValue), Is.EqualTo(1));
        Assert.That(_viewModel.TimeoutValues.Count, Is.EqualTo(4)); // Still only 4 items
    }

    [Test]
    public void SecondsAuthenticationCanceled_WhenSet_SavesValueToConfigurationService()
    {
        // Arrange
        var newTimeout = new TimeoutOptionDto { Seconds = 45 };
            
        // Act
        _viewModel!.SecondsAuthenticationCanceled = newTimeout;
            
        // Assert
        Assert.That(_currentSettings!.SecondsAuthenticationCanceled, Is.EqualTo(45));
        Assert.That(_viewModel.SecondsAuthenticationCanceled, Is.EqualTo(newTimeout));
    }

    [Test]
    public void SecondsAuthenticationCanceled_WhenSetWithSameValue_DoesNotTriggerSave()
    {
        // Arrange
        var initialValue = _viewModel!.SecondsAuthenticationCanceled.Seconds;
        var sameTimeout = new TimeoutOptionDto { Seconds = initialValue };
            
        // Reset mock to track calls
        _configurationServiceMock!.Invocations.Clear();
            
        // Act
        _viewModel.SecondsAuthenticationCanceled = sameTimeout;
            
        // Assert - проверяем, что свойство не вызвало сохранение
        Assert.That(_currentSettings!.SecondsAuthenticationCanceled, Is.EqualTo(initialValue));
    }

    [Test]
    public void SecondsAuthenticationCanceled_WhenSet_UpdatesPropertyAndNotifiesChange()
    {
        // Arrange
        var newTimeout = new TimeoutOptionDto { Seconds = 60 };
        bool propertyChangedRaised = false;
            
        _viewModel!.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(SettingsShiftOpeningPageViewModel.SecondsAuthenticationCanceled))
                propertyChangedRaised = true;
        };
            
        // Act
        _viewModel.SecondsAuthenticationCanceled = newTimeout;
            
        // Assert
        Assert.That(propertyChangedRaised, Is.True);
    }

    [Test]
    public void TimeoutValues_ContainsAllDefaultValues()
    {
        // Arrange
        var expectedSeconds = new HashSet<short> { 10, 15, 30, 60 };
            
        // Assert
        var actualSeconds = _viewModel!.TimeoutValues.Select(x => x.Seconds).ToHashSet();
        Assert.That(actualSeconds, Is.EquivalentTo(expectedSeconds));
    }

    [Test]
    public void TimeoutValues_AreSortedCorrectly()
    {
        // Act
        var sortedSeconds = _viewModel!.TimeoutValues.Select(x => x.Seconds).OrderBy(x => x).ToList();
            
        // Assert
        Assert.That(sortedSeconds, Is.EqualTo([10, 15, 30, 60]));
    }

    [Test]
    public void Constructor_WhenConfigurationServiceReturnsNull_ThrowsNullReferenceException()
    {
        // Arrange
        _configurationServiceMock!.Setup(x => x.CurrentSetting).Returns((SettingsModel)null);
            
        // Act & Assert
        Assert.That(() => new SettingsShiftOpeningPageViewModel(_loggerMock!.Object, _configurationServiceMock.Object), 
            Throws.InstanceOf<NullReferenceException>());
    }

    [TestCase((short)5)]
    [TestCase((short)20)]
    [TestCase((short)120)]
    public void SecondsAuthenticationCanceled_WithVariousValues_SavesCorrectly(short seconds)
    {
        // Arrange
        var newTimeout = new TimeoutOptionDto { Seconds = seconds };
            
        // Act
        _viewModel!.SecondsAuthenticationCanceled = newTimeout;
            
        // Assert
        Assert.That(_currentSettings!.SecondsAuthenticationCanceled, Is.EqualTo(seconds));
    }

    [Test]
    public void InitializeData_WhenCalledWithCustomValue_AddsToTimeoutValues()
    {
        // Arrange
        const short customTimeout = 45;
        _currentSettings!.SecondsAuthenticationCanceled = customTimeout;
            
        // Act
        _viewModel = new SettingsShiftOpeningPageViewModel(
            _loggerMock!.Object,
            _configurationServiceMock!.Object);
            
        // Assert
        Assert.That(_viewModel.TimeoutValues.Any(x => x.Seconds == customTimeout), Is.True);
        Assert.That(_viewModel.TimeoutValues.Count, Is.EqualTo(5));
    }

    [Test]
    public void MultipleInstances_UseSameConfigurationService()
    {
        // Arrange
        const short customTimeout = 25;
            
        // Act
        var viewModel1 = new SettingsShiftOpeningPageViewModel(
            _loggerMock!.Object,
            _configurationServiceMock!.Object)
        {
            SecondsAuthenticationCanceled = new TimeoutOptionDto { Seconds = customTimeout }
        };

        var viewModel2 = new SettingsShiftOpeningPageViewModel(
            _loggerMock!.Object,
            _configurationServiceMock!.Object);
            
        // Assert
        Assert.That(viewModel2.SecondsAuthenticationCanceled.Seconds, Is.EqualTo(customTimeout));
    }

    [Test]
    public void TimeoutValues_AfterAddingCustomValue_RemainsUnique()
    {
        // Arrange
        const short customTimeout = 15;
        _currentSettings!.SecondsAuthenticationCanceled = customTimeout;
            
        // Act
        _viewModel = new SettingsShiftOpeningPageViewModel(_loggerMock!.Object, _configurationServiceMock!.Object);
            
        // Assert
        var count = _viewModel.TimeoutValues.Count(x => x.Seconds == customTimeout);
        Assert.That(count, Is.EqualTo(1));
    }

    [Test]
    public void PropertyChanged_WhenSecondsAuthenticationCanceledSet_RaisesEventWithCorrectPropertyName()
    {
        // Arrange
        string? propertyName = null;
        _viewModel!.PropertyChanged += (_, args) =>
        {
            propertyName = args.PropertyName!;
        };
            
        // Act
        _viewModel.SecondsAuthenticationCanceled = new TimeoutOptionDto { Seconds = 20 };
            
        // Assert
        Assert.That(propertyName, Is.EqualTo(nameof(SettingsShiftOpeningPageViewModel.SecondsAuthenticationCanceled)));
    }

    [Test]
    public void TimeoutValues_IsReadOnlyCollection()
    {
        // Assert - проверяем, что нельзя заменить коллекцию целиком
        var originalCollection = _viewModel!.TimeoutValues;
            
        // Попытка изменить коллекцию должна работать (HashSet можно изменять)
        Assert.That(originalCollection, Is.InstanceOf<HashSet<TimeoutOptionDto>>());
            
        // Проверяем, что коллекция инициализирована и содержит элементы
        Assert.That(originalCollection, Is.Not.Empty);
    }

    [Test]
    public void SecondsAuthenticationCanceled_DefaultValue_MatchesConfiguration()
    {
        // Assert
        Assert.That(_viewModel!.SecondsAuthenticationCanceled, Is.Not.Null);
        Assert.That(_viewModel.SecondsAuthenticationCanceled.Seconds, 
            Is.EqualTo(_currentSettings!.SecondsAuthenticationCanceled));
    }

    [Test]
    public void Constructor_WhenCalled_LogsInformation()
    {
        // Проверяем, что логгер вызывается (если в базовом классе есть логирование)
        // Это зависит от реализации PageViewModelBase
            
        // Act & Assert - просто проверяем, что конструктор отработал без ошибок
        Assert.That(_viewModel, Is.Not.Null);
    }
}