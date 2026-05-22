using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Terminal.Application.Implementations.Builders;
using Terminal.Application.Interfaces.DbEntitiesServices;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.DbEntities.MainDb;
using Terminal.Core.Enums;
using Terminal.Persistence.MainDB;

namespace Terminal.UnitTests.Services;

[TestFixture]
public class SellingBuilderTests
{
    private SellingBuilder? _builder;
    private Mock<ILogger<SellingBuilder>> _mockLogger = null!;
    private Mock<IShiftService> _shiftServiceMock = null!;
    private Mock<IDbContextFactory<DataContext>> _dbFactoryMock = null!;
    private Mock<IParameterService> _paramDbFactoryMock = null!;

    [SetUp]
    public void SetUp()
    {
        _mockLogger = new Mock<ILogger<SellingBuilder>>();
        _shiftServiceMock = new Mock<IShiftService>();
        _dbFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        _paramDbFactoryMock = new Mock<IParameterService>();
        
        _builder = new SellingBuilder(
            _mockLogger.Object,
            _shiftServiceMock.Object,
            _dbFactoryMock.Object,
            _paramDbFactoryMock.Object);
    }

    [Test]
    public void SetPaymentTypes_SetsBaseAndDerivedTypes()
    {
        // Arrange
        const BasePaymentType baseType = BasePaymentType.Cash;
        const DerivedPaymentType derivedType = DerivedPaymentType.FuelCard;

        // Act
        _builder!.SetPaymentTypes(baseType, derivedType);
        var result = _builder!.Build();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.BaseType, Is.EqualTo(baseType));
            Assert.That(result.DerivedType, Is.EqualTo(derivedType));
        });
    }

    [Test]
    public void SetResourceCode_SetsResourceProperties()
    {
        // Arrange
        var resourceCode = new ResourceCode
        {
            ResourceKey = 100,
            ResourceName = "Test Resource",
            ResourcePrice = 250.50m
        };

        // Act
        _builder!.SetResourceCode(resourceCode);
        var result = _builder!.Build();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.ResourceKey, Is.EqualTo(resourceCode.ResourceKey));
            Assert.That(result.ResourceCode, Is.EqualTo(resourceCode.ResourceKey));
            Assert.That(result.ResourceName, Is.EqualTo(resourceCode.ResourceName));
            Assert.That(result.SellingPrice, Is.EqualTo(resourceCode.ResourcePrice));
        });
    }

    [Test]
    public void SetAmount_SetsAmountProperty()
    {
        // Arrange
        const decimal amount = 5.5m;

        // Act
        _builder!.SetAmount(amount);
        var result = _builder!.Build();

        // Assert
        Assert.That(result.Amount, Is.EqualTo(amount));
    }

    [Test]
    public async Task SetCheckNumber_SetsCheckNumberProperty()
    {
        // Arrange
        var expectedCheckNumber = 101;
        
        var settings = new List<Setting>
        {
            new() { SettingsKey = SettingsKey.Sale, Value = 100 }
        }.AsQueryable();
    
        var mockDbSet = new Mock<DbSet<Setting>>();
        mockDbSet.As<IQueryable<Setting>>().Setup(m => m.Provider).Returns(settings.Provider);
        mockDbSet.As<IQueryable<Setting>>().Setup(m => m.Expression).Returns(settings.Expression);
        mockDbSet.As<IQueryable<Setting>>().Setup(m => m.ElementType).Returns(settings.ElementType);
        mockDbSet.As<IQueryable<Setting>>().Setup(m => m.GetEnumerator()).Returns(settings.GetEnumerator());
        
        mockDbSet.Setup(x => x.FindAsync(It.IsAny<object[]>()))
            .ReturnsAsync(settings.First());
    
        var mockDbContext = new Mock<DataContext>();
        mockDbContext.Setup(x => x.Settings).Returns(mockDbSet.Object);
    
        _dbFactoryMock.Setup(x => x.CreateDbContextAsync())
            .ReturnsAsync(mockDbContext.Object);
    
        // Act
        await _builder!.SetCheckNumber();
        var result = _builder!.Build();
    
        // Assert
        Assert.That(result.CheckNumber, Is.EqualTo(expectedCheckNumber));
        mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void SetPersonKey_SetsPersonKeyAndName()
    {
        // Arrange
        const int personKey = 42;
        const string personName = "operator_1";

        // Act
        _builder!.SetPersonKey(personKey, personName);
        var result = _builder!.Build();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.PersonKey, Is.EqualTo(personKey));
            Assert.That(result.PersonName, Is.EqualTo(personName));
        });
    }

    [Test]
    public void SetPersonKey_WithNullPersonName_SetsPersonKeyOnly()
    {
        // Arrange
        const int personKey = 42;
        string? personName = null;

        // Act
        _builder!.SetPersonKey(personKey, personName);
        var result = _builder!.Build();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.PersonKey, Is.EqualTo(personKey));
            Assert.That(result.PersonName, Is.Null);
        });
    }

    [Test]
    public void Build_SetsTransactionDatetimeToCurrentTime()
    {
        // Arrange
        var beforeBuild = DateTime.Now;

        // Act
        var result = _builder!.Build();

        // Assert
        Assert.That(result.TransactionDatetime, Is.GreaterThanOrEqualTo(beforeBuild));
        Assert.That(result.TransactionDatetime, Is.LessThanOrEqualTo(DateTime.Now));
    }

    [Test]
    public void Build_CalculatesShopCostCorrectly()
    {
        // Arrange
        var resourceCode = new ResourceCode
        {
            ResourceKey = 1,
            ResourceName = "Test",
            ResourcePrice = 100.00m
        };
        const decimal amount = 3.5m;
        const decimal expectedShopCost = 350.00m; 
        
        _builder!.SetResourceCode(resourceCode);
        _builder!.SetAmount(amount);

        // Act
        var result = _builder!.Build();

        // Assert
        Assert.That(result.ShopCost, Is.EqualTo(expectedShopCost));
    }

    [Test]
    public void SetRequestedVolume_WhenIsCostTrue_SetsRequestedCostAndCalculatesRequestedAmount()
    {
        // Arrange
        const string volume = "500,50";
        const bool isCost = true;
        const decimal amount = 10.00m;
        const decimal expectedRequestedCost = 500.50m;
        const decimal expectedRequestedAmount = 50.05m;

        _builder!.SetAmount(amount);

        // Act
        _builder!.SetRequestedVolume(volume, isCost);
        var result = _builder!.Build();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.RequestedCost, Is.EqualTo(expectedRequestedCost));
            Assert.That(result.RequestedAmount, Is.EqualTo(expectedRequestedAmount).Within(0.001m));
        });
    }

    [Test]
    public void SetRequestedVolume_WhenIsCostFalse_SetsRequestedAmountAndCalculatesRequestedCost()
    {
        // Arrange
        const string volume = "25,750";
        const bool isCost = false;
        const decimal amount = 2.50m;
        const decimal expectedRequestedAmount = 25.750m;
        const decimal expectedRequestedCost = 10.30m;

        _builder!.SetAmount(amount);

        // Act
        _builder!.SetRequestedVolume(volume, isCost);
        var result = _builder!.Build();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.RequestedAmount, Is.EqualTo(expectedRequestedAmount));
            Assert.That(result.RequestedCost, Is.EqualTo(expectedRequestedCost).Within(0.01m));
        });
    }

    [Test]
    public void SetRequestedVolume_WithIntegerVolume_SetsCorrectValues()
    {
        // Arrange
        const string volume = "100";
        const bool isCost = true;
        const decimal amount = 20.00m;
        const decimal expectedRequestedCost = 100.00m;
        const decimal expectedRequestedAmount = 5.00m;

        _builder!.SetAmount(amount);

        // Act
        _builder!.SetRequestedVolume(volume, isCost);
        var result = _builder!.Build();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.RequestedCost, Is.EqualTo(expectedRequestedCost));
            Assert.That(result.RequestedAmount, Is.EqualTo(expectedRequestedAmount));
        });
    }

    [Test]
    public void SetRequestedVolume_WithZeroAmount_ThrowsDivideByZeroException()
    {
        // Arrange
        const string volume = "100";
        const bool isCost = true;

        _builder!.SetAmount(0);

        // Act & Assert
        Assert.That(() => _builder!.SetRequestedVolume(volume, isCost),
            Throws.InstanceOf<DivideByZeroException>());
    }

    [Test]
    public void SetRequestedVolume_WithNegativeVolume_SetsNegativeValues()
    {
        // Arrange
        const string volume = "-50,75";
        const bool isCost = true;
        const decimal amount = 10.00m;
        const decimal expectedRequestedCost = -50.75m;
        const decimal expectedRequestedAmount = -5.075m;

        _builder!.SetAmount(amount);

        // Act
        _builder!.SetRequestedVolume(volume, isCost);
        var result = _builder!.Build();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.RequestedCost, Is.EqualTo(expectedRequestedCost));
            Assert.That(result.RequestedAmount, Is.EqualTo(expectedRequestedAmount).Within(0.001m));
        });
    }

    [TestCase("100,50")]
    [TestCase("100,5")]
    [TestCase("100")]
    [TestCase("0,01")]
    public void SetRequestedVolume_WithVariousVolumeFormats_ParsesCorrectly(string volume)
    {
        // Arrange
        const bool isCost = true;
        const decimal amount = 10.00m;
        _builder!.SetAmount(amount);

        // Act
        Assert.That(() => _builder!.SetRequestedVolume(volume, isCost), Throws.Nothing);
        var result = _builder!.Build();

        // Assert
        Assert.That(result.RequestedCost, Is.Not.Null);
    }

    [Test]
    public void SetRequestedVolume_RoundsRequestedCostToTwoDecimals()
    {
        // Arrange
        const string volume = "100,555";
        const bool isCost = true;
        const decimal amount = 10.00m;
        const decimal expectedRequestedCost = 100.56m;

        _builder!.SetAmount(amount);

        // Act
        _builder!.SetRequestedVolume(volume, isCost);
        var result = _builder!.Build();

        // Assert
        Assert.That(result.RequestedCost, Is.EqualTo(expectedRequestedCost));
    }

    [Test]
    public void SetRequestedVolume_RoundsRequestedAmountToThreeDecimals()
    {
        // Arrange
        const string volume = "100,5555";
        const bool isCost = false;
        const decimal amount = 10.00m;
        const decimal expectedRequestedAmount = 100.556m;

        _builder!.SetAmount(amount);

        // Act
        _builder!.SetRequestedVolume(volume, isCost);
        var result = _builder!.Build();

        // Assert
        Assert.That(result.RequestedAmount, Is.EqualTo(expectedRequestedAmount).Within(0.0001m));
    }

    [Test]
    public void SetRequestedVolume_WithCultureInvariantFormat_UsesRussianCulture()
    {
        // Arrange
        var originalCulture = Thread.CurrentThread.CurrentCulture;
        try
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
            const string volume = "100,50";
            const bool isCost = true;
            const decimal amount = 10.00m;
            const decimal expectedRequestedCost = 100.50m;

            _builder!.SetAmount(amount);

            // Act
            _builder!.SetRequestedVolume(volume, isCost);
            var result = _builder!.Build();

            // Assert
            Assert.That(result.RequestedCost, Is.EqualTo(expectedRequestedCost));
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = originalCulture;
        }
    }

    [Test]
public async Task Build_AfterMultipleSetOperations_ReturnsCompleteSelling()
{
    // Arrange
    var resourceCode = new ResourceCode
    {
        ResourceKey = 200,
        ResourceName = "Premium Service",
        ResourcePrice = 150.00m
    };
    const decimal amount = 2m;
    const int expectedCheckNumber = 101;
    const int personKey = 100;
    const string personName = "Jane Smith";
    const string volume = "300,00";
    const bool isCost = true;
    
    var options = new DbContextOptionsBuilder<DataContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

    await using var dbContext = new DataContext(options);
    
    dbContext.Settings.Add(new Setting { SettingsKey = SettingsKey.Sale, Value = 100 });
    await dbContext.SaveChangesAsync();
    
    _dbFactoryMock.Setup(x => x.CreateDbContextAsync())
        .ReturnsAsync(dbContext);
    
    // Act
    _builder!.SetPaymentTypes(BasePaymentType.Cash, DerivedPaymentType.FuelCard);
    _builder!.SetResourceCode(resourceCode);
    _builder!.SetAmount(amount);
    await _builder!.SetCheckNumber();
    _builder!.SetPersonKey(personKey, personName);
    _builder!.SetRequestedVolume(volume, isCost);
    var result = _builder!.Build();
    
    // Assert
    Assert.Multiple(() =>
    {
        Assert.That(result.BaseType, Is.EqualTo(BasePaymentType.Cash));
        Assert.That(result.DerivedType, Is.EqualTo(DerivedPaymentType.FuelCard));
        Assert.That(result.ResourceKey, Is.EqualTo(resourceCode.ResourceKey));
        Assert.That(result.ResourceCode, Is.EqualTo(resourceCode.ResourceKey));
        Assert.That(result.ResourceName, Is.EqualTo(resourceCode.ResourceName));
        Assert.That(result.SellingPrice, Is.EqualTo(resourceCode.ResourcePrice));
        Assert.That(result.Amount, Is.EqualTo(amount));
        Assert.That(result.CheckNumber, Is.EqualTo(expectedCheckNumber));
        Assert.That(result.PersonKey, Is.EqualTo(personKey));
        Assert.That(result.PersonName, Is.EqualTo(personName));
        Assert.That(result.RequestedCost, Is.EqualTo(300.00m));
        Assert.That(result.RequestedAmount, Is.EqualTo(150.00m));
        Assert.That(result.ShopCost, Is.EqualTo(300.00m));
        Assert.That(result.TransactionDatetime, Is.Not.EqualTo(default(DateTime)));
    });
}

    [Test]
    public void SetRequestedVolume_WhenCalledMultipleTimes_OverwritesPreviousValues()
    {
        // Arrange
        const decimal amount = 10.00m;
        _builder!.SetAmount(amount);

        // Act
        _builder!.SetRequestedVolume("100,00", true);
        _builder!.SetRequestedVolume("200,00", true);
        var result = _builder!.Build();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.RequestedCost, Is.EqualTo(200.00m));
            Assert.That(result.RequestedAmount, Is.EqualTo(20.00m));
        });
    }

    [Test]
    public void SetResourceCode_WhenCalledMultipleTimes_OverwritesPreviousResource()
    {
        // Arrange
        var firstResource = new ResourceCode
        {
            ResourceKey = 1,
            ResourceName = "First",
            ResourcePrice = 100
        };
        var secondResource = new ResourceCode
        {
            ResourceKey = 2,
            ResourceName = "Second",
            ResourcePrice = 200
        };

        // Act
        _builder!.SetResourceCode(firstResource);
        _builder!.SetResourceCode(secondResource);
        var result = _builder!.Build();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.ResourceKey, Is.EqualTo(2));
            Assert.That(result.ResourceName, Is.EqualTo("Second"));
            Assert.That(result.SellingPrice, Is.EqualTo(200));
        });
    }

    [Test]
    public void Build_WithoutSettingAmount_ShopCostIsZero()
    {
        // Arrange
        var resourceCode = new ResourceCode { ResourcePrice = 100 };
        _builder!.SetResourceCode(resourceCode);

        // Act
        var result = _builder!.Build();

        // Assert
        Assert.That(result.ShopCost, Is.Null);
    }

    [Test]
    public void Build_WithoutSettingResourceCode_ShopCostIsZero()
    {
        // Arrange
        _builder!.SetAmount(5);

        // Act
        var result = _builder!.Build();

        // Assert
        Assert.That(result.ShopCost, Is.Null);
    }

    [Test]
    public void SetRequestedVolume_WithVeryLargeVolume_HandlesCorrectly()
    {
        // Arrange
        const string volume = "999999999,99";
        const bool isCost = true;
        const decimal amount = 1m;
        const decimal expectedRequestedCost = 999999999.99m;

        _builder!.SetAmount(amount);

        // Act
        _builder!.SetRequestedVolume(volume, isCost);
        var result = _builder!.Build();

        // Assert
        Assert.That(result.RequestedCost, Is.EqualTo(expectedRequestedCost));
        Assert.That(result.RequestedAmount, Is.EqualTo(expectedRequestedCost / amount));
    }

    [Test]
    public void SetRequestedVolume_WithVerySmallVolume_HandlesCorrectly()
    {
        // Arrange
        const string volume = "0,001";
        const bool isCost = false;
        const decimal amount = 100m;
        const decimal expectedRequestedAmount = 0.001m;

        _builder!.SetAmount(amount);

        // Act
        _builder!.SetRequestedVolume(volume, isCost);
        var result = _builder!.Build();

        // Assert
        Assert.That(result.RequestedAmount, Is.EqualTo(expectedRequestedAmount).Within(0.0001m));
        Assert.That(result.RequestedCost, Is.EqualTo(0.00001m).Within(0.00001m));
    }
}