using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Terminal.Application.Implementations.DbEntitiesServices;
using Terminal.Core.DbEntities;
using Terminal.Core.Enums;
using Terminal.Core.Models;
using Terminal.ViewModels.Pages;

namespace Terminal.IntegrationTests.PagesTests;

[TestFixture]
public class SaleProcessPageTests : IntegrationTestsBase
{
    private SaleProcessPageViewModel? _saleProcessPageViewModel;

    [SetUp]
    public void InitField()
    {
        _saleProcessPageViewModel = TestScope!.ServiceProvider.GetRequiredService<SaleProcessPageViewModel>();
        _saleProcessPageViewModel.OnActivated(NavigationMock.Object);
    }
    
    [Test]
    public async Task FullSaleWithCashPass()
    {
        // Arrange
        await using var db = await DbFactory!.CreateDbContextAsync();

        var resource = await db.ResourceCodes.FirstAsync(x => x.IsShow == 1);
        var charsAmount = new[] {"0", "1", "2", "3", ",", "00", "1" , "1"};

        var shift = new Shift
        {
            ShiftKey = 1,
            ShopKey = 9,
            IsOpened = true,
            ShiftDate = DateTime.Now
        };

        await db.Shifts.AddAsync(shift);
        await db.SaveChangesAsync();
        
        ReceiptPrintMock
            .Setup(x => x.PrintSalesReceiptAsync(It.IsAny<SalesReceipt>()))
            .ReturnsAsync(new PrintResult { Success = true, Status = PrinterStatus.Ready });
        
        // Act
        _saleProcessPageViewModel!.SetFuelType(resource!);

        _saleProcessPageViewModel.IsAmountMoney = false;
        foreach (var item in charsAmount)
            _saleProcessPageViewModel.AddCharInAmountPreview(item);
        
        _saleProcessPageViewModel.SetAmount();
        
        await _saleProcessPageViewModel!.SetPaymentType("Наличные");
        
        // Assert
        var creationSelling = await db.Sales.OrderByDescending(x => x.TransactionShopKey).FirstAsync();
        
        Assert.That(creationSelling.Amount == (decimal)123.001);
        Assert.That(creationSelling.ResourceCode == resource.ResourceKey);
    }
}