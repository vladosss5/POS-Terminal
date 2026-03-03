using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MsBox.Avalonia;
using Terminal.Application.Interfaces.Builders;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.DbEntities;
using Terminal.Core.Enums;
using Terminal.Core.Models;
using Terminal.Data.Context;
using Terminal.ViewModels.Items;

namespace Terminal.ViewModels.Pages;

/// <summary>
/// Страница процесса заправки по карте.
/// </summary>
public partial class RefuelingByCardPageViewModel : PageViewModelBase
{
    /// Фабрика создающая <inheritdoc cref="DataContext"/>
    private readonly IDbContextFactory<DataContext> _dbFactory;

    /// <inheritdoc cref="IPrintService"/>
    private readonly IPrintService _printService;
    
    /// <inheritdoc cref="ISellingBuilder"/>
    private readonly ISellingBuilder _builder;
    
    /// <inheritdoc cref="ILogger"/>
    private readonly ILogger<RefuelingByCardPageViewModel> _logger;
    
    private readonly CultureInfo _russianCulture;
    
    /// <summary>
    /// Кол-во топлива.
    /// </summary>
    private decimal _amountFuel;

    /// <summary>
    /// Сообщения о типах кол-ва.
    /// </summary>
    private string[] _amountMessages = new []
    {
        "Указывается кол-во в ₽",
        "Указывается кол-во в литрах"
    };
    
    /// <summary>
    /// Коллекция шагов заправки.
    /// </summary>
    [ObservableProperty] private ObservableCollection<StepViewModelBase> _steps;

    /// <summary>
    /// Индекс текущего шага.
    /// </summary>
    [ObservableProperty] private int _currentStepIndex;
    
    /// <summary>
    /// Процесс начат?
    /// </summary>
    [ObservableProperty] private bool _isProcessStarted;
    
    /// <summary>
    /// Типы оплаты.
    /// </summary>
    [ObservableProperty] private PaymentTypes? _selectedCardType;
    
    /// <summary>
    /// Выбранный тип топлива (товар).
    /// </summary>
    [ObservableProperty] private ResourceCode? _selectedFuelType;

    /// <summary>
    /// Кол-во указано в деньгах?
    /// Если нет, то в литрах.
    /// </summary>
    [ObservableProperty] private bool _isAmountMoney = true;

    /// <summary>
    /// Сообщение-указатель на единицу измерения для пользователя.
    /// </summary>
    [ObservableProperty] private string _amountWhat;

    /// <summary>
    /// Наименование текущей страницы (шага).
    /// </summary>
    [ObservableProperty] private string _nameCurrentPage;

    /// <summary>
    /// Св-во для хранения товаров (типов топлива).
    /// </summary>
    [ObservableProperty] private ObservableCollection<ResourceCode> _resources;
    
    
    /// <summary>
    /// Св-во для хранения типов оплаты.
    /// </summary>
    public IEnumerable<PaymentTypes> PaymentTypes => Enum.GetValues<PaymentTypes>();

    /// <summary>
    /// Коллекция цифровых кнопок. 
    /// </summary>
    public ObservableCollection<string> KeypadButtons { get; } = new()
    { 
        "7",  "8", "9" , 
        "4",  "5", "6" , 
        "1",  "2", "3" , 
        "00", "0", ","
    };
    
    /// <summary>
    /// Предпросмотр кол-ва денег.
    /// </summary>
    public string AmountMoneyPreview
    {
        get => string.IsNullOrEmpty(field) ? "0" : field;
        set
        {
            if (!SetProperty(ref field, value)) 
                return;
            
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out var d))
                _amountFuel = d / (SelectedFuelType?.ResourcePrice ?? 1m);
        }
    }

    /// <summary>
    /// Предпросмотр кол-ва топлива.
    /// </summary>
    public string AmountFuelPreview
    {
        get => string.IsNullOrEmpty(field) ? "0" : field;
        set
        {
            if (!SetProperty(ref field, value)) 
                return;
            
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out var d))
                _amountFuel = d;
        }
    }
    

    /// <summary>
    /// Конструктор.
    /// </summary>
    public RefuelingByCardPageViewModel(
        ISellingBuilder builder, 
        IDbContextFactory<DataContext> dbFactory, 
        ILogger<RefuelingByCardPageViewModel> logger, 
        IPrintService printService) 
        : base(logger)
    {
        _builder = builder;
        _dbFactory = dbFactory;
        _logger = logger;
        _printService = printService;
        _russianCulture = new CultureInfo("ru-RU");

        InitializeSteps();
        _ = LoadDataAsync();
        
        IsProcessStarted = true;
        CurrentStepIndex = 0;
        Steps[0].IsActive = true;
        
        _amountWhat = IsAmountMoney ? _amountMessages[0] : _amountMessages[1];
    }


    /// <summary>
    /// Указать тип оплаты.
    /// </summary>
    /// <param name="type">Тип оплаты.</param>
    public void SetPaymentType(PaymentTypes type)
    {
        _builder.SetPaymentType(type);

        SelectedCardType = type;
        Steps[0].CompleteStepCommand.Execute(null);
    }

    /// <summary>
    /// Указать тип топлива (товара).
    /// </summary>
    /// <param name="type">Топливо.</param>
    public void SetFuelType(ResourceCode type)
    {
        _builder.SetResourceCode(type.FuelCodeKey);

        SelectedFuelType = type;
        Steps[1].CompleteStepCommand.Execute(null);
    }
    
    /// <summary>
    /// Указать кол-во.
    /// </summary>
    public void SetCount()
    {
        _builder.SetAmount(_amountFuel);

        Steps[2].CompleteStepCommand.Execute(null);
    }
    
    /// <summary>
    /// Перейти к прошлому шагу.
    /// </summary>
    public void StepBack()
    {
        if (CurrentStepIndex > 0)
        {
            Steps[CurrentStepIndex].IsActive = false;
        
            CurrentStepIndex--;
            NameCurrentPage = Steps[CurrentStepIndex].StepName;
        
            var prevStep = Steps[CurrentStepIndex];
            prevStep.IsActive = true;
            prevStep.IsCompleted = false;
        }
        else
        {
            Navigation.GoBack();
        }
    }

    /// <summary>
    /// Добавить символ в превьювер кол-ва.
    /// </summary>
    /// <param name="item">Символ.</param>
    public void AddCharInAmountPreview(string item)
    {
        string current = IsAmountMoney ? AmountMoneyPreview : AmountFuelPreview;
        int maxDecimals = IsAmountMoney ? 2 : 3;

        int dotIndex = current.IndexOf(",", StringComparison.Ordinal);
        
        if (dotIndex >= 0 && item != ",")
        {
            int decimalsAfterDot = current.Length - dotIndex - 1;
            
            if (decimalsAfterDot >= maxDecimals) 
                return;
        }
        
        
        if (item == "," && current.Contains(item))
            return;

        if (current == "0" && item == ",")
            current = "0";

        string newValue = current == "0" && item != ","
            ? item
            : current + item;
        
        if (newValue.Length > 14) 
            return;

        if (IsAmountMoney)
            AmountMoneyPreview = newValue;
        else
            AmountFuelPreview = newValue;
    }

    /// <summary>
    /// Удалить последний символ из превьювера кол-ва.
    /// </summary>
    ///
    public void DeleteLastCharFromPreview()
    {
        if (IsAmountMoney)
        {
            AmountMoneyPreview = DeleteLastChar(AmountMoneyPreview);
        }
        else
        {
            AmountFuelPreview = DeleteLastChar(AmountFuelPreview);
        }
    }
    
    private string DeleteLastChar(string str) 
    {
        if (str.Length > 1)
        {
            str = str[..^1];
        }
        else
        {
            str = "0";
        }

        return str;
    }

    /// <summary>
    /// Сбросить значение превьювера на 0.
    /// </summary>
    public void AmountPreviewSetZero()
    {
        AmountFuelPreview = "0";
        AmountMoneyPreview = "0";
    }

    /// <summary>
    /// Сменить единицу измерения (деньги на литры).
    /// </summary>
    public void SwitchAmount()
    {
        if (IsAmountMoney)
        {
            if (!decimal.TryParse(AmountMoneyPreview, NumberStyles.Any, _russianCulture, out var money))
                return;
            
            _amountFuel = money / (SelectedFuelType?.ResourcePrice ?? 1m);
            AmountFuelPreview = _amountFuel
                .ToString($"N3", _russianCulture)
                .TrimEnd('0')
                .TrimEnd(',');
                
            IsAmountMoney = false;
            AmountWhat = _amountMessages[1];
        }
        else
        {
            if (!decimal.TryParse(AmountFuelPreview, NumberStyles.Any, _russianCulture, out var fuel)) 
                return;
            
            AmountMoneyPreview = (fuel * (SelectedFuelType?.ResourcePrice ?? 1m))
                .ToString($"N2", _russianCulture)
                .TrimEnd('0')
                .TrimEnd(',');
                
            IsAmountMoney = true;
            AmountWhat = _amountMessages[0];
        }
    }

    /// <summary>
    /// Подгрузка данных из БД.
    /// </summary>
    private async Task LoadDataAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        
        var products = await db.ResourceCodes
            .Where(x => x.IsShow == 1)
            .OrderBy(p => p.ResourceName)
            .AsNoTracking()
            .ToArrayAsync();

        Resources = new ObservableCollection<ResourceCode>(products);
    }

    /// <summary>
    /// Инициализировать шаги покупки.
    /// </summary>
    private void InitializeSteps()
    {
        Steps = new ObservableCollection<StepViewModelBase>
        {
            new("Тип оплаты", OnStepCompleted),
            new("Тип топлива", OnStepCompleted),
            new("Количество", OnStepCompleted)
        };

        NameCurrentPage = Steps[0].StepName;
    }

    /// <summary>
    /// Пометить шаг выполненным.
    /// </summary>
    private async Task OnStepCompleted()
    {
        if (CurrentStepIndex < Steps.Count - 1)
        {
            CurrentStepIndex++;
            NameCurrentPage = Steps[CurrentStepIndex].StepName;
            Steps[CurrentStepIndex].IsActive = true;
        }
        else
        {
            await CompleteRefuelingProcess();
        }
    }
    
    /// <summary>
    /// Завершить процесс заправки по карте.
    /// </summary>
    private async Task CompleteRefuelingProcess()
    {
        try
        {
            var selling = _builder.Build();
            
            await using var db = await _dbFactory.CreateDbContextAsync();

            await db.AddAsync(selling);
            await db.SaveChangesAsync();

            await PrintReceiptAsync(selling);
            
            await ShowMessage("Успех!", $"Сделана покупка №{selling.TransactionShopKey}");
            
            Navigation.GoBack();    
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"Ошибка: {ex.Message}, {ex.StackTrace}" );
            await ShowMessage("Ошибка!", $"{ex.Message}, {ex.StackTrace}");
        }
    }

    private async Task PrintReceiptAsync(Selling selling)
    {
        if (!_printService.IsConnected)
            await _printService.ConnectAsync();
        
        var receipe = new SalesReceipt
        {
            Selling = selling,
            Total = selling.ParcelPrice is null ? 0 : (decimal)selling.ParcelPrice
        };
        
        var printResult = await _printService.PrintSalesReceiptAsync(receipe);
        
        _logger.LogInformation($"Чек отбит.\n Результаты печати: {printResult.Status}, {printResult.ErrorMessage}");
        
        if (_printService.IsConnected)
            _printService.Disconnect();
    }

    private async Task ShowMessage(string title, string text)
    {
        _logger.LogInformation($"{title}: {text}");
        
        await MessageBoxManager
            .GetMessageBoxStandard(title, text)
            .ShowAsync();
    }
}