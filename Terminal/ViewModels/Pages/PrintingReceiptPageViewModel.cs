using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaEdit.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Mappers;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.DbEntities.MainDb;
using Terminal.Data.MainDB;
using Terminal.ViewModels.Items;

namespace Terminal.ViewModels.Pages;

/// <summary>
/// Логика работы страницы печати чека PrintingReceiptPageView.
/// </summary>
public partial class PrintingReceiptPageViewModel : PageViewModelBase
{
    /// Фабрика экземпляров: <inheritdoc cref="DataContext"/>
    private readonly IDbContextFactory<DataContext> _dbFactory;

    /// <inheritdoc cref="IReceiptPrintService" />
    private readonly IReceiptPrintService _printService;

    /// <inheritdoc cref="ISalesReceiptMappingService" />
    private readonly ISalesReceiptMappingService _receiptMappingService;
    
    private readonly CultureInfo _culture = CultureInfo.InvariantCulture;

    private readonly CultureInfo _cultureForDate;

    /// <summary>
    /// Кол-во элементов на странице.
    /// </summary>
    private const int PageSize = 20;
    
    /// <summary>
    /// Кол-во пропускаемых записей при подгрузке.
    /// </summary>
    private int _currentSkip;
    
    /// <summary>
    /// Токен отмены для работы с БД.
    /// </summary>
    private CancellationTokenSource? _loadingCts;
    
    /// <summary>
    /// Вернёт true если есть ли ещё не загруженные данные/
    /// </summary>
    [ObservableProperty] private bool _hasMoreItems = true;
    
    /// <summary>
    /// Вернёт true, если идёт загрузка данных.
    /// </summary>
    [ObservableProperty] private bool _isLoading;

    /// <summary>
    /// Коллекция чеков.
    /// </summary>
    public ObservableCollection<ReceiptForListingDto> SalesPerShiftCollection { get; } = [];

    /// <summary>
    /// Ключевое слово для поиска.
    /// </summary>
    public string Keyword
    {
        get;
        set 
        {
            if (SetProperty(ref field, value))
                _ = OnKeywordChangedAsync();
        }
    }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public PrintingReceiptPageViewModel(
        ILogger<PageViewModelBase> logger, 
        IDbContextFactory<DataContext> dbFactory, 
        IReceiptPrintService printService, 
        ISalesReceiptMappingService receiptMappingService) 
        : base(logger)
    {
        _dbFactory = dbFactory;
        _printService = printService;
        _receiptMappingService = receiptMappingService;

        _cultureForDate = new CultureInfo("ru-RU");

        _ = LoadMoreReceiptsAsync();
    }

    /// <summary>
    /// Распечатать выбранный чек.
    /// </summary>
    /// <param name="receiptDto">Чек.</param>
    public async Task PrintReceipt(ReceiptForListingDto receiptDto)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        
        var selling = await db.Sales.FirstOrDefaultAsync(x => x.CheckNumber == receiptDto.CheckNumber);
        if (selling == null)
        {
            Logger.LogError("Продажа не найдена!");
            return;
        }
        
        var receipt = _receiptMappingService.MapSellingToSalesReceipt(selling);
        var printResult = await _printService.PrintSalesReceiptAsync(receipt);
        
        Logger.LogInformation($"Чек отбит.\n Результаты печати: {printResult.Status}, {printResult.ErrorMessage}");
    }
    
    /// <summary>
    /// Загрузить ещё чеков.
    /// </summary>
    public async Task LoadMoreReceiptsAsync(CancellationToken cancellationToken = default)
    {
        if (!HasMoreItems)
            return;
        
        try
        {
            IsLoading = true;

            await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);

            var sales = await db.Sales
                .OrderByDescending(x => x.TransactionShopKey)
                .Skip(_currentSkip)
                .Take(PageSize + 1)
                .ToListAsync(cancellationToken);
            
            cancellationToken.ThrowIfCancellationRequested();

            var hasMore = sales.Count > PageSize;
            if (hasMore)
                sales.RemoveAt(PageSize);
            
            var receipts = MapSellingToReceiptForListingDtos(sales);
            SalesPerShiftCollection.AddRange(receipts);
            
            _currentSkip += sales.Count;
            HasMoreItems = hasMore;
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    /// <summary>
    /// Обрабатывает изменение поискового запроса.
    /// </summary>
    private async Task OnKeywordChangedAsync()
    {
        _loadingCts?.Cancel();
        _loadingCts = new CancellationTokenSource();
        var token = _loadingCts.Token;

        try 
        {
            await Task.Delay(500, token);
            
            IsLoading = true;

            if (string.IsNullOrWhiteSpace(Keyword))
                await ResetToPagedModeAsync(token);
            else
                await LoadAllFilteredAsync(token);
            
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    /// <summary>
    /// Сбрасывает режим к бесконечной прокрутке (без фильтра).
    /// </summary>
    private async Task ResetToPagedModeAsync(CancellationToken token)
    {
        SalesPerShiftCollection.Clear();
        _currentSkip = 0;
        HasMoreItems = true;
        await LoadMoreReceiptsAsync(token);
    }

    /// <summary>
    /// Загрузка записей из БД содержащих Keyword в подстроке.
    /// </summary>
    private async Task LoadAllFilteredAsync(CancellationToken token)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(token);

        var keyword = Keyword.Trim();
        var query = db.Sales.AsNoTracking().AsQueryable();
        
        var pattern = $"%{keyword}%";
        var numericKeyword = long.TryParse(keyword, out var num) ? num : (long?)null;
        
        query = query.Where(s =>
            (numericKeyword != null && s.CheckNumber == numericKeyword) ||
            (s.ResourceName != null && EF.Functions.Like(s.ResourceName, pattern))
        );
        
        var dtos = await query
            .OrderByDescending(s => s.TransactionDatetime)
            .Select(s => new ReceiptForListingDto
            {
                CheckNumber = s.CheckNumber ?? s.TransactionShopKey,
                ResourceName = s.ResourceName ?? "Неизвестный товар",
                ResourceCount = s.Amount.HasValue ? Convert.ToDecimal(s.Amount.Value, _culture) : 0m,
                PricePerItem = s.SellingPrice.HasValue ? Convert.ToDecimal(s.SellingPrice.Value, _culture) : 0m,
                SaleDate = s.TransactionDatetime == null
                    ? DateTime.MinValue.ToString(_cultureForDate)
                    : s.TransactionDatetime.Value.ToString(_cultureForDate),
                FullReceiptPrice = s.ShopCost.HasValue ? Convert.ToDecimal(s.ShopCost.Value, _culture) : 0m
            })
            .ToListAsync(token);

        SalesPerShiftCollection.Clear();
        SalesPerShiftCollection.AddRange(dtos);
        
        HasMoreItems = false;
    }
    
    /// <summary>
    /// Перейти к прошлому шагу.
    /// </summary>
    public void StepBack() => Navigation!.GoBack();

    /// <summary>
    /// Маппинг коллекций доменной модели Selling к коллекции отображаемых элементов. 
    /// </summary>
    /// <param name="sales">Перечисление объектов Selling.</param>
    /// <returns>Перечисление отображаемых объектов.</returns>
    private IEnumerable<ReceiptForListingDto> MapSellingToReceiptForListingDtos(IEnumerable<Selling> sales)
    {
        var salesList = sales.ToList();
        
        if (salesList.Count == 0)
            return Enumerable.Empty<ReceiptForListingDto>();
        
        return salesList.Select(sale => new ReceiptForListingDto
        {
            CheckNumber = sale.CheckNumber ?? sale.TransactionShopKey,
            ResourceName = sale.ResourceName ?? sale.ResourceName ?? "Неизвестный товар",
            ResourceCount = sale.Amount.HasValue ? Convert.ToDecimal(sale.Amount.Value, _culture) : 0m,
            PricePerItem = sale.SellingPrice.HasValue ? Convert.ToDecimal(sale.SellingPrice.Value, _culture) : 0m,
            SaleDate = sale.TransactionDatetime == null 
                ? DateTime.MinValue.ToString(_cultureForDate) 
                : sale.TransactionDatetime.Value.ToString(_cultureForDate),
            FullReceiptPrice = sale.ShopCost.HasValue ? Convert.ToDecimal(sale.ShopCost.Value, _culture) : 0m
        });
    }
}