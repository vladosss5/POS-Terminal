using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Terminal.Core.Enums;
using Terminal.Core.Models;
using Terminal.ViewModels.Pages;

namespace Terminal.IntegrationTests.PagesTests;

[TestFixture]
public class SaleProcessPageTests : IntegrationTestsBase
{
    private SaleProcessPageViewModel? _saleProcessPageViewModel;

    [SetUp]
    public new void SetUp()
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