using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using AvaloniaEdit.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.DbEntities.MainDb;
using Terminal.Core.Enums;
using Terminal.Core.Models;
using Terminal.Data.MainDB;

namespace Terminal.ViewModels.Pages;

/// <summary>
/// Страница изменения цены ресурсов.
/// </summary>
public class ResourcePageViewModel : PageViewModelBase
{
    /// Фабрика создающая <inheritdoc cref="DataContext"/>
    private readonly IDbContextFactory<DataContext> _dbFactory;
    
    /// <inheritdoc cref="IParameterService" />
    private readonly IParameterService _parameterService;

    ///<inheritdoc cref="IReceiptPrintService"/>
    private readonly IReceiptPrintService _receiptPrintService;
    
    ///<inheritdoc cref="IAuthService"/>
    private readonly IAuthService _authService;

    private static readonly CultureInfo CultureForNumbers = CultureInfo.InvariantCulture;

    /// <summary>
    /// Стандартное название страницы.
    /// </summary>
    private const string DefaultTitle = "Смена цены";

    /// <summary>
    /// Коллекция кнопок для авторизации.
    /// </summary>
    public string[] NumberButtons { get; } =
    [
        "7", "8", "9",
        "4", "5", "6",
        "1", "2", "3",
        "00", "0", ".",
    ];

    /// <summary>
    /// Предпросмотр цены.
    /// </summary>
    public string? PricePreview
    {
        get; set => SetProperty(ref field, value);
    }

    /// <summary>
    /// Ресурс выбран? True - если выбран.
    /// </summary>
    public bool ResourceHasBeenSelected
    {
        get; set => SetProperty(ref field, value);
    }

    /// <summary>
    /// Выбранный для редактирования ресурс.
    /// </summary>
    public ResourceCode? SelectedResourceCode
    {
        get;
        private set
        {
            if (!SetProperty(ref field, value)) 
                return;
            
            if (value == null) 
                return;
            
            ResourceHasBeenSelected = true;
            Title = value.ResourceName ?? string.Empty;
        }
    }

    /// <summary>
    /// Коллекция ресурсов.
    /// </summary>
    public ObservableCollection<ResourceCode> Resources
    {
        get; set => SetProperty(ref field, value);
    } = [];
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public ResourcePageViewModel(
        ILogger<PageViewModelBase> logger, 
        IDbContextFactory<DataContext> dbFactory, 
        IReceiptPrintService receiptPrintService, 
        IAuthService authService, 
        IParameterService parameterService) 
        : base(logger)
    {
        Title = DefaultTitle;
        _dbFactory = dbFactory;
        _receiptPrintService = receiptPrintService;
        _authService = authService;
        _parameterService = parameterService;

        _ = LoadData();
    }

    /// <summary>
    /// Выбрать ресурс для редактирования.
    /// </summary>
    /// <param name="resource">Редактируемый ресурс.</param>
    public void SelectResource(ResourceCode resource)
    {
        SelectedResourceCode = resource;
        PricePreview = resource.ResourcePrice != null 
            ? resource.ResourcePrice.Value.ToString(CultureInfo.InvariantCulture)
            : "0";
    }

    /// <summary>
    /// Шаг назад.
    /// </summary>
    public void StepBack()
    {
        if (!ResourceHasBeenSelected)
        {
            Navigation!.NavigateTo<MainMenuPageViewModel>();
            return;
        }
        
        ResourceHasBeenSelected = false;
        Title = DefaultTitle;
        SelectedResourceCode = null;
    }

    /// <summary>
    /// Сохранить цену.
    /// </summary>
    public async Task SavePrice()
    {
        if (SelectedResourceCode == null)
            return;

        await using var db = await _dbFactory.CreateDbContextAsync();

        var resourceCode =
            await db.ResourceCodes.FirstOrDefaultAsync(x => x.ResourceKey == SelectedResourceCode!.ResourceKey);
        if (resourceCode == null)
            return;

        if (!decimal.TryParse(PricePreview, NumberStyles.Any, CultureForNumbers, out var newPrice))
            return;

        if (resourceCode.ResourcePrice!.Value == newPrice)
            ResetPageData();

        var oldValue = resourceCode.ResourcePrice;

        resourceCode.ResourcePrice = newPrice;

        db.Update(resourceCode);
        await db.SaveChangesAsync();

        var index = Resources.IndexOf(SelectedResourceCode);

        if (index < 0)
            return;

        await Print(oldValue, newPrice);

        SelectedResourceCode.ResourcePrice = newPrice;
        Resources[index] = SelectedResourceCode;

        ResetPageData();
    }

    /// <summary>
    /// Добавить символ к паролю.
    /// </summary>
    /// <param name="symbols">Символ.</param>
    public void AddCharInPassword(string symbols)
    {
        if (PricePreview == null)
            return;
        
        foreach (var symbol in symbols)
        {
            var dotIndex = PricePreview.IndexOf('.');
        
            if (dotIndex >= 0 && symbol != '.')
            {
                var decimalsAfterDot = PricePreview.Length - dotIndex - 1;
                if (decimalsAfterDot >= 2)
                    return;
            }
        
            if (symbol == '.' && PricePreview.Contains(symbol))
                return;

            if (PricePreview == "0" && symbol == '.')
                PricePreview = "0";

            string newValue;

            if (PricePreview == "0" && symbol != '.')
                newValue = symbol.ToString();
            else
                newValue = PricePreview + symbol;
        
            if (newValue.Length > 14)
                return;

            PricePreview = newValue;
        }
    }
    
    /// <summary>
    /// Стереть последний символ.
    /// </summary>
    public void RemoveLastChar()
    {
        if (string.IsNullOrWhiteSpace(PricePreview))
            return;
        
        PricePreview = PricePreview[..^1];
    }

    /// <summary>
    /// Очистить пароль.
    /// </summary>
    public void ClearPassword() => PricePreview = string.Empty;
    
    /// <summary>
    /// Подгрузка данных при инициализации страницы.
    /// </summary>
    private async Task LoadData()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        var resources = await db.ResourceCodes.ToListAsync();
        
        Resources.AddRange(resources);
    }
    
    /// <summary>
    /// Сброс данных на странице.
    /// </summary>
    private void ResetPageData()
    {
        SelectedResourceCode = null;
        ResourceHasBeenSelected = false;
        Title = DefaultTitle;
    }
    
    /// <summary>
    /// Печать чека о смене цены.
    /// </summary>
    /// <param name="oldValue">Цена до.</param>
    /// <param name="newValue">Цена после.</param>
    private async Task Print(decimal? oldValue, decimal newValue)
    {
        var issuerNumber = await _parameterService.GetValueAsync(AppParameter.IssuerId);
        var terminalNumber = await _parameterService.GetValueAsync(AppParameter.SerialNO111);
        var operatorName = _authService.CurrentUser?.Name;
        
        var changeData = new PriceChangeData
        {
            IssuerNumber = issuerNumber,
            TerminalNumber = terminalNumber,
            ChangingDateTime = DateTime.Now,
            ResourceName = SelectedResourceCode != null ? SelectedResourceCode.ResourceName! : "undefined",
            PriceUpTo = oldValue ?? 0,
            PriceAfter = newValue,
            OperatorName = operatorName ?? "undefined"
        };
        
        await _receiptPrintService.PrintPriceChangeAsync(changeData);
    }
}