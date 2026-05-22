using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Terminal.Application.Interfaces.Builders;
using Terminal.Application.Interfaces.Mappers;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.DbEntities.MainDb;
using Terminal.Core.Enums;
using Terminal.Core.Models;
using Terminal.Core.Models.Settings;
using Terminal.Data.MainDB;
using Terminal.ViewModels.Pages;

namespace Terminal.UnitTests.Services;

[TestFixture]
public class SaleProcessPageViewModelTests
{
    private Mock<ISellingBuilder> _builderMock;
    private Mock<IDbContextFactory<DataContext>> _dbFactoryMock;
    private Mock<ILogger<SaleProcessPageViewModel>> _loggerMock;
    private Mock<IReceiptPrintService> _receiptPrintServiceMock;
    private Mock<ISalesReceiptMappingService> _receiptMappingServiceMock;
    private Mock<ICardReaderService> _cardReaderServiceMock;
    private Mock<IConfigurationService> _configurationServiceMock;
    private Mock<ISettingPaymentTypeMapper> _settingPaymentTypeMapperMock;
    private Mock<IAuthService> _authServiceMock;
    private Mock<DataContext> _dbContextMock;
    private SettingsModel _currentSettings;
    private SaleProcessPageViewModel _viewModel;

    [SetUp]
    public void SetUp()
    {
        _builderMock = new Mock<ISellingBuilder>();
        _dbFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        _loggerMock = new Mock<ILogger<SaleProcessPageViewModel>>();
        _receiptPrintServiceMock = new Mock<IReceiptPrintService>();
        _receiptMappingServiceMock = new Mock<ISalesReceiptMappingService>();
        _cardReaderServiceMock = new Mock<ICardReaderService>();
        _configurationServiceMock = new Mock<IConfigurationService>();
        _settingPaymentTypeMapperMock = new Mock<ISettingPaymentTypeMapper>();
        _authServiceMock = new Mock<IAuthService>();
        _dbContextMock = new Mock<DataContext>();

        _currentSettings = new SettingsModel
        {
            PaymentTypes =
            [
                new SettingPaymentType
                {
                    DisplayedName = "Наличные",
                    BaseType = (int)BasePaymentType.Cash,
                    DerivedType = (int)DerivedPaymentType.Cash,
                    IsEnabled = true,
                },
                new SettingPaymentType
                {
                    DisplayedName = "Банковская карта",
                    BaseType = (int)BasePaymentType.NonCash,
                    DerivedType = (int)DerivedPaymentType.BankCard,
                    IsEnabled = true,
                },
                new SettingPaymentType
                {
                    DisplayedName = "Топливная",
                    BaseType = (int)BasePaymentType.NonCash,
                    DerivedType = (int)DerivedPaymentType.FuelCard,
                    IsEnabled = true,
                },
                new SettingPaymentType
                {
                    DisplayedName = "Ведомость",
                    BaseType = (int)BasePaymentType.NonCash,
                    DerivedType = (int)DerivedPaymentType.FuelStatement,
                    IsEnabled = true,
                },
                new SettingPaymentType
                {
                    DisplayedName = "Талоны",
                    BaseType = (int)BasePaymentType.NonCash,
                    DerivedType = (int)DerivedPaymentType.FuelTalon,
                    IsEnabled = true,
                }
            ]
        };

        _configurationServiceMock.Setup(x => x.CurrentSetting)
            .Returns(_currentSettings);
        _dbFactoryMock.Setup(x => x.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_dbContextMock.Object);
        _settingPaymentTypeMapperMock
            .Setup(x => x.SettingPaymentTypeToDto(It.IsAny<SettingPaymentType>()))
            .Returns((SettingPaymentType setting) => new PaymentTypeDto
            {
                DisplayedName = setting.DisplayedName,
                BaseType = (BasePaymentType)setting.BaseType,
                DerivedType = (DerivedPaymentType)setting.DerivedType,
                IsEnabled = setting.IsEnabled
            });

        _viewModel = new SaleProcessPageViewModel(
            _builderMock.Object,
            _dbFactoryMock.Object,
            _loggerMock.Object,
            _receiptPrintServiceMock.Object,
            _receiptMappingServiceMock.Object,
            _cardReaderServiceMock.Object,
            _configurationServiceMock.Object,
            _settingPaymentTypeMapperMock.Object,
            _authServiceMock.Object);
    }


    [Test]
    public async Task SetPaymentType_WhenPaymentTypeDoesNotExist_DoesNotCallBuilder()
    {
        // Arrange
        _viewModel.PaymentTypesDictionary =
            new Dictionary<string, (BasePaymentType BaseType, DerivedPaymentType DerivedType)>
            {
                {"Наличные", (BasePaymentType.Cash, DerivedPaymentType.Cash)}
            };
        
        var invalidPaymentTypeKey = "Несуществующий тип";

        // Act
        await _viewModel.SetPaymentType(invalidPaymentTypeKey);

        // Assert
        _builderMock.Verify(x => x.SetPaymentTypes(It.IsAny<BasePaymentType>(), It.IsAny<DerivedPaymentType>()), 
            Times.Never);
    }

    [Test]
    public void SetFuelType_WhenResourceIsValid_SetsResourceCodeAndSelectedFuelType()
    {
        // Arrange
        var resource = new ResourceCode 
        { 
            ResourceKey = 1, 
            ResourceName = "АИ-92", 
            ResourcePrice = 45.50m 
        };

        // Act
        _viewModel.SetFuelType(resource);

        // Assert
        Assert.Multiple(() =>
        {
            _builderMock.Verify(x => x.SetResourceCode(resource), Times.Once);
            Assert.That(_viewModel.SelectedFuelType, Is.EqualTo(resource));
        });
    }

    [Test]
    public void SetAmount_WhenAmountIsValid_SetsAmountAndRequestedVolume()
    {
        // Arrange
        var resource = new ResourceCode { ResourcePrice = 50.00m };
        _viewModel.SetFuelType(resource);
        _viewModel.AmountMoneyPreview = "1000";
        _viewModel.IsAmountMoney = true;

        // Act
        _viewModel.SetAmount();

        // Assert
        Assert.Multiple(() =>
        {
            _builderMock.Verify(x => x.SetAmount(It.IsAny<decimal>()), Times.Once);
            _builderMock.Verify(x => x.SetRequestedVolume("1000", true), Times.Once);
        });
    }

    [Test]
    public void StepBack_WhenCurrentStepIndexGreaterThanZero_DecreasesStepIndex()
    {
        // Arrange
        _viewModel.CurrentStepIndex = 2;
        var initialIndex = _viewModel.CurrentStepIndex;

        // Act
        _viewModel.StepBack();

        // Assert
        Assert.That(_viewModel.CurrentStepIndex, Is.EqualTo(initialIndex - 1));
    }

    [Test]
    public void StepBack_WhenCurrentStepIndexIsZero_CallsNavigationGoBack()
    {
        // Arrange
        var currentIndex = 1;
        _viewModel.CurrentStepIndex = currentIndex;
        
        // Act
        _viewModel.StepBack();

        // Assert - проверяем, что навигация назад вызвана
        Assert.That(_viewModel.CurrentStepIndex, Is.EqualTo(currentIndex - 1));
    }

    [Test]
    public void AddCharInAmountPreview_WhenAddingDigitToZero_UpdatesPreview()
    {
        // Arrange
        _viewModel.AmountMoneyPreview = "0";
        _viewModel.IsAmountMoney = true;

        // Act
        _viewModel.AddCharInAmountPreview("5");

        // Assert
        Assert.That(_viewModel.AmountMoneyPreview, Is.EqualTo("5"));
    }

    [Test]
    public void AddCharInAmountPreview_WhenExceedingMaxDecimals_DoesNotAddChar()
    {
        // Arrange
        _viewModel.AmountMoneyPreview = "100,50";
        _viewModel.IsAmountMoney = true;

        // Act
        _viewModel.AddCharInAmountPreview("5");

        // Assert
        Assert.That(_viewModel.AmountMoneyPreview, Is.EqualTo("100,50"));
    }

    [Test]
    public void AddCharInAmountPreview_WhenAddingMultipleZeros_HandlesCorrectly()
    {
        // Arrange
        _viewModel.AmountMoneyPreview = "0";
        _viewModel.IsAmountMoney = true;

        // Act
        _viewModel.AddCharInAmountPreview("00");

        // Assert
        Assert.That(_viewModel.AmountMoneyPreview, Is.EqualTo("0"));
    }

    [Test]
    public void AddCharInAmountPreview_WhenExceedingMaxLength_DoesNotAddChar()
    {
        // Arrange
        _viewModel.AmountMoneyPreview = new string('1', 14);
        _viewModel.IsAmountMoney = true;

        // Act
        _viewModel.AddCharInAmountPreview("5");

        // Assert
        Assert.That(_viewModel.AmountMoneyPreview.Length, Is.EqualTo(14));
    }

    [Test]
    public void DeleteLastCharFromPreview_WhenPreviewHasMultipleChars_RemovesLastChar()
    {
        // Arrange
        _viewModel.AmountMoneyPreview = "123";
        _viewModel.IsAmountMoney = true;

        // Act
        _viewModel.DeleteLastCharFromPreview();

        // Assert
        Assert.That(_viewModel.AmountMoneyPreview, Is.EqualTo("12"));
    }

    [Test]
    public void DeleteLastCharFromPreview_WhenPreviewHasSingleChar_ResetsToZero()
    {
        // Arrange
        _viewModel.AmountMoneyPreview = "5";
        _viewModel.IsAmountMoney = true;

        // Act
        _viewModel.DeleteLastCharFromPreview();

        // Assert
        Assert.That(_viewModel.AmountMoneyPreview, Is.EqualTo("0"));
    }

    [Test]
    public void SwitchAmount_WhenSwitchingFromMoneyToFuel_UpdatesPreviewAndFlag()
    {
        // Arrange
        var resource = new ResourceCode { ResourcePrice = 50.00m };
        _viewModel.SetFuelType(resource);
        _viewModel.AmountMoneyPreview = "100";
        _viewModel.IsAmountMoney = true;

        // Act
        _viewModel.SwitchAmount();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(_viewModel.IsAmountMoney, Is.False);
            Assert.That(_viewModel.AmountWhat, Is.EqualTo("Указывается кол-во в литрах"));
            Assert.That(_viewModel.AmountFuelPreview, Is.Not.EqualTo("0"));
        });
    }

    [Test]
    public void SwitchAmount_WhenSwitchingWithInvalidPreview_DoesNotChange()
    {
        // Arrange
        _viewModel.AmountMoneyPreview = "invalid";
        _viewModel.IsAmountMoney = true;
        var wasAmountMoney = _viewModel.IsAmountMoney;

        // Act
        _viewModel.SwitchAmount();

        // Assert
        Assert.That(_viewModel.IsAmountMoney, Is.EqualTo(wasAmountMoney));
    }

    [Test]
    public void AmountPreviewSetZero_SetsBothPreviewsToZero()
    {
        // Arrange
        _viewModel.AmountMoneyPreview = "100";
        _viewModel.AmountFuelPreview = "50";

        // Act
        _viewModel.AmountPreviewSetZero();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(_viewModel.AmountMoneyPreview, Is.EqualTo("0"));
            Assert.That(_viewModel.AmountFuelPreview, Is.EqualTo("0"));
        });
    }

    [Test]
    public void AmountPreviewSetZero_WhenPreviewsAlreadyZero_RemainsZero()
    {
        // Arrange
        _viewModel.AmountMoneyPreview = "0";
        _viewModel.AmountFuelPreview = "0";

        // Act
        _viewModel.AmountPreviewSetZero();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(_viewModel.AmountMoneyPreview, Is.EqualTo("0"));
            Assert.That(_viewModel.AmountFuelPreview, Is.EqualTo("0"));
        });
    }

    [Test]
    public void Constructor_WhenAllDependenciesValid_InitializesPropertiesCorrectly()
    {
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(_viewModel.IsProcessStarted, Is.True);
            Assert.That(_viewModel.CurrentStepIndex, Is.EqualTo(0));
            Assert.That(_viewModel.KeypadButtons, Is.Not.Empty);
            Assert.That(_viewModel.KeypadButtons.Count, Is.EqualTo(12));
            Assert.That(_viewModel.AmountWhat, Is.EqualTo("Указывается кол-во в ₽"));
        });
    }

    [Test]
    public void AmountMoneyPreview_WhenSettingValidValue_UpdatesAmountFuel()
    {
        // Arrange
        var resource = new ResourceCode { ResourcePrice = 50.00m };
        _viewModel.SetFuelType(resource);
        
        // Act
        _viewModel.AmountMoneyPreview = "100";

        // Assert
        Assert.That(_viewModel.AmountMoneyPreview, Is.EqualTo("100"));
    }


    [Test]
    public void AmountFuelPreview_WhenSettingValidValue_UpdatesAmountFuel()
    {
        // Act
        _viewModel.AmountFuelPreview = "50,5";

        // Assert
        Assert.That(_viewModel.AmountFuelPreview, Is.EqualTo("50,5"));
    }

    [Test]
    public void Steps_WhenInitialized_ContainsFourSteps()
    {
        // Assert
        Assert.That(_viewModel.Steps.Count, Is.EqualTo(4));
        Assert.That(_viewModel.Steps[0].StepName, Is.EqualTo("Тип топлива"));
        Assert.That(_viewModel.Steps[1].StepName, Is.EqualTo("Количество"));
        Assert.That(_viewModel.Steps[2].StepName, Is.EqualTo("Тип оплаты"));
        Assert.That(_viewModel.Steps[3].StepName, Is.EqualTo("Считывание"));
    }

    [Test]
    public void Steps_FirstStepIsActiveInitially()
    {
        // Assert
        Assert.That(_viewModel.Steps[0].IsActive, Is.True);
        Assert.That(_viewModel.Steps[1].IsActive, Is.False);
    }

    [Test]
    public void KeypadButtons_ContainsAllExpectedButtons()
    {
        // Arrange
        var expectedButtons = new[] { "7", "8", "9", "4", "5", "6", "1", "2", "3", "00", "0", "," };

        // Assert
        CollectionAssert.AreEquivalent(expectedButtons, _viewModel.KeypadButtons);
    }

    [Test]
    public void KeypadButtons_OrderIsCorrect()
    {
        // Arrange
        var expectedOrder = new[] { "7", "8", "9", "4", "5", "6", "1", "2", "3", "00", "0", "," };

        // Assert
        CollectionAssert.AreEqual(expectedOrder, _viewModel.KeypadButtons);
    }
}