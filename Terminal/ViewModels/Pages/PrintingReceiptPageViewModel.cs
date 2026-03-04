using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaEdit.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.DbEntities;
using Terminal.Core.Models;
using Terminal.Data.Context;
using Terminal.ViewModels.Items;

namespace Terminal.ViewModels.Pages;

/// <summary>
/// Логика работы страницы печати чека PrintingReceiptPageView.
/// </summary>
public class PrintingReceiptPageViewModel : PageViewModelBase
{
    /// Фабрика экземпляров: <inheritdoc cref="DataContext"/>
    private readonly IDbContextFactory<DataContext> _dbFactory;

    /// <inheritdoc cref="IPrintService" />
    private readonly IPrintService _printService;

    /// <summary>
    /// Коллекция чеков.
    /// </summary>
    public ObservableCollection<ReceiptForListingDto> SalesPerShiftCollection { get; } = new();

    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public PrintingReceiptPageViewModel(
        ILogger<PageViewModelBase> logger, 
        IDbContextFactory<DataContext> dbFactory, 
        IPrintService printService) 
        : base(logger)
    {
        _dbFactory = dbFactory;
        _printService = printService;

        _ = LoadDataAsync();
    }

    /// <summary>
    /// Распечатать выбранный чек.
    /// </summary>
    /// <param name="receiptDto">Чек.</param>
    public async Task PrintReceipt(ReceiptForListingDto receiptDto)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        var selling = await db.Sales.FirstOrDefaultAsync(x => x.TransactionShopKey == receiptDto.TransactionShopKey);

        if (!_printService.IsConnected)
            await _printService.ConnectAsync();
        
        var receipt = new SalesReceipt
        {
            Selling = selling,
            Total = selling.ParcelPrice is null ? 0 : (decimal)selling.ParcelPrice
        };
        
        var printResult = await _printService.PrintSalesReceiptAsync(receipt);
        
        Logger.LogInformation($"Чек отбит.\n Результаты печати: {printResult.Status}, {printResult.ErrorMessage}");
        
        if (_printService.IsConnected)
            _printService.Disconnect();
    }
    
    /// <summary>
    /// Перейти к прошлому шагу.
    /// </summary>
    public void StepBack() => Navigation.GoBack();

    /// <summary>
    /// Подгрузить данные.
    /// </summary>
    private async Task LoadDataAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        var sales = await db.Sales
            .OrderByDescending(x => x.TransactionShopKey)
            .ToListAsync();
        
        var receipts = MapSellingToReceiptForListingDtos(sales);
        
        SalesPerShiftCollection.AddRange(receipts);
    }

    /// <summary>
    /// Маппинг коллекций доменной модели Selling к коллекции отображаемых элементов. 
    /// </summary>
    /// <param name="sales">Перечисление объектов Selling.</param>
    /// <returns>Перечисление отображаемых объектов.</returns>
    private IEnumerable<ReceiptForListingDto> MapSellingToReceiptForListingDtos(IEnumerable<Selling> sales)
    {
        if (sales == null)
            return Enumerable.Empty<ReceiptForListingDto>();
        
        return sales.Select(sale => new ReceiptForListingDto
        {
            TransactionShopKey = sale.TransactionShopKey,
            ResourceName = sale.ResourceName ?? sale.ResourceName ?? "Неизвестный товар",
            ResourceCount = sale.Amount.HasValue ? Convert.ToDecimal(sale.Amount.Value) : 0m,
            PricePerItem = sale.SellingPrice.HasValue ? Convert.ToDecimal(sale.SellingPrice.Value) : 0m,
            SaleDate = sale.TransactionDatetime ?? DateTime.MinValue,
            FullReceiptPrice = sale.ShopCost.HasValue ? Convert.ToDecimal(sale.ShopCost.Value) : 0m
        });
    }
}