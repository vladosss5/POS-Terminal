using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MsBox.Avalonia;
using Terminal.Application.Interfaces.Builders;
using Terminal.Application.Interfaces.Mappers;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.DbEntities;
using Terminal.Core.Enums;
using Terminal.Data.Context;
using Terminal.ViewModels.Items;

namespace Terminal.ViewModels.Pages;

/// <summary>
/// Страница процесса заправки по карте.
/// </summary>
public partial class SaleProcessPageViewModel : PageViewModelBase
{
    /// Фабрика создающая <inheritdoc cref="DataContext"/>
    private readonly IDbContextFactory<DataContext> _dbFactory;

    /// <inheritdoc cref="IReceiptPrintService"/>
    private readonly IReceiptPrintService _receiptPrintService;
    
    /// <inheritdoc cref="ISellingBuilder"/>
    private readonly ISellingBuilder _builder;
    
    /// <inheritdoc cref="ILogger"/>
    private readonly ILogger<SaleProcessPageViewModel> _logger;
    
    /// <inheritdoc cref="ISalesReceiptMappingService" />
    private readonly ISalesReceiptMappingService _receiptMappingService;

    /// <inheritdoc cref="ICardReaderService" />
    private readonly ICardReaderService _cardReaderService;

    /// <inheritdoc cref="IConfigurationService" />
    private readonly IConfigurationService _configurationService;
    
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
    /// Токен отмены считывания карты.
    /// </summary>
    private CancellationTokenSource? _cardReadCts;

    /// <summary>
    /// Индекс текущего шага.
    /// </summary>
    [ObservableProperty] private int _currentStepIndex;
    
    /// <summary>
    /// Процесс начат?
    /// </summary>
    [ObservableProperty] private bool _isProcessStarted;
    
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
    /// Коллекция шагов заправки.
    /// </summary>
    public ObservableCollection<StepViewModelBase> Steps
    {
        get;
        private set => SetProperty(ref field, value);
    }
    
    /// <summary>
    /// Типы оплаты.
    /// </summary>
    public Dictionary<string, (BasePaymentType BaseType, DerivedPaymentType DerivedType)> PaymentTypesDictionary { get; } = new();

    /// <summary>
    /// Коллекция цифровых кнопок. 
    /// </summary>
    public ObservableCollection<string> KeypadButtons { get; } =
    [
        "7", "8", "9",
        "4", "5", "6",
        "1", "2", "3",
        "00", "0", ","
    ];
    
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
            
            if (decimal.TryParse(value, NumberStyles.Any, new CultureInfo("ru-RU"), out var d))
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
            
            if (decimal.TryParse(value, NumberStyles.Any, new CultureInfo("ru-RU"), out var d))
                _amountFuel = d;
        }
    }

    
    /// <summary>
    /// Строковая информация по карте.
    /// </summary>
    public string CardInfo
    {
        get;
        private set => SetProperty(ref field, value);
    }
    

    /// <summary>
    /// Конструктор.
    /// </summary>
    public SaleProcessPageViewModel(
        ISellingBuilder builder, 
        IDbContextFactory<DataContext> dbFactory, 
        ILogger<SaleProcessPageViewModel> logger, 
        IReceiptPrintService receiptPrintService, 
        ISalesReceiptMappingService receiptMappingService, 
        ICardReaderService cardReaderService, IConfigurationService configurationService) 
        : base(logger)
    {
        _builder = builder;
        _dbFactory = dbFactory;
        _logger = logger;
        _receiptPrintService = receiptPrintService;
        _receiptMappingService = receiptMappingService;
        _cardReaderService = cardReaderService;
        _configurationService = configurationService;
        _russianCulture = new CultureInfo("ru-RU");

        InitializeSteps();
        InitializePaymentTypes();
        _ = LoadDataAsync();
        
        IsProcessStarted = true;
        CurrentStepIndex = 0;
        Steps![0].IsActive = true;
        
        _amountWhat = IsAmountMoney ? _amountMessages[0] : _amountMessages[1];
    }


    /// <summary>
    /// Указать тип оплаты.
    /// </summary>
    /// <param name="typeKey">Тип оплаты.</param>
    public async Task SetPaymentType(string typeKey)
    {
        if (!PaymentTypesDictionary.TryGetValue(typeKey, out var value)) 
            return;

        _builder.SetPaymentTypes(value.BaseType, value.DerivedType);
        Steps[2].CompleteStepCommand.Execute(null);
        
        if (value.DerivedType == DerivedPaymentType.BankCard)
        {
            try
            {
                await ProcessCardForPaymentAsync();
            }
            catch(Exception e)
            {
                _logger.LogError($"{e.Message}, {e.InnerException}");
            }
        }
    }

    /// <summary>
    /// Указать тип топлива (товара).
    /// </summary>
    /// <param name="resource">Топливо.</param>
    public void SetFuelType(ResourceCode resource)
    {
        _builder.SetResourceCode(resource);

        SelectedFuelType = resource;
        Steps[0].CompleteStepCommand.Execute(null);
    }
    
    /// <summary>
    /// Указать кол-во топлива.
    /// </summary>
    public void SetAmount()
    {
        _builder.SetAmount(_amountFuel);
        
        var volume = IsAmountMoney ? AmountMoneyPreview : AmountFuelPreview;
        _builder.SetRequestedVolume(volume, IsAmountMoney);
        
        Steps[1].CompleteStepCommand.Execute(null);
    }
    
    /// <summary>
    /// Перейти к прошлому шагу.
    /// </summary>
    public void StepBack()
    {
        if (CurrentStepIndex > 0)
        {
            if (CurrentStepIndex == 3)
                _cardReadCts?.Cancel();
            
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
    /// Добавить символ в предпросмотр кол-ва.
    /// </summary>
    /// <param name="item">Символ.</param>
    public void AddCharInAmountPreview(string item)
    {
        if (item == "00")
        {
            AddCharInAmountPreview("0");
            AddCharInAmountPreview("0");
            return;
        }
        
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
        
        var newValue = current == "0" && item != ","
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
    /// Удалить последний символ из предпросмотра кол-ва.
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

    /// <summary>
    /// Сбросить значения предпросмотров на 0.
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
        Steps =
        [
            new StepViewModelBase("Тип топлива", OnStepCompleted),
            new StepViewModelBase("Количество", OnStepCompleted),
            new StepViewModelBase("Тип оплаты", OnStepCompleted),
            new StepViewModelBase("Считывание", OnStepCompleted)
        ];

        NameCurrentPage = Steps[0].StepName;
    }

    /// <summary>
    /// Подгрузить методы оплаты из конфигурации.
    /// </summary>
    private void InitializePaymentTypes()
    {
        var paymentTypeList = _configurationService.GetPaymentTypeSettings();

        if (paymentTypeList == null)
        {
            _logger.LogError("Не удалось загрузить список типов оплат.");
            return;
        }
        
        foreach (var paymentType in paymentTypeList)
        {
            if (!paymentType.IsEnabled)
                continue;
            
            PaymentTypesDictionary.Add(paymentType.DisplayedName, (paymentType.BaseType, paymentType.DerivedType));
        }
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
            await using var db = await _dbFactory.CreateDbContextAsync();

            var chekNumberSetting = await db.Settings.FindAsync(SettingsKey.Sale);

            if (chekNumberSetting == null)
                return;
            
            var currentNumber = chekNumberSetting.Value!.Value + 1;
            
            _builder.SetCheckNumber(currentNumber);
            var selling = _builder.Build();
            await db.AddAsync(selling);

            chekNumberSetting.Value = currentNumber;
            db.Update(chekNumberSetting);
            
            await db.SaveChangesAsync();

            await PrintReceiptAsync(selling);
            
            Navigation.GoBack();    
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка: {ex.Message},\n {ex.InnerException}, \n {ex.StackTrace}" );
            await ShowMessage("Ошибка!", $"{ex.Message}, {ex.InnerException}");
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
    /// Запуск процесса оплаты по карте.
    /// </summary>
    private async Task ProcessCardForPaymentAsync()
    {
        if (_cardReadCts != null)
            await _cardReadCts?.CancelAsync()!;
        
        _cardReadCts = new CancellationTokenSource();

        try
        {
            var result = await _cardReaderService.ReadCardAsync(
                timeoutSeconds: 30,
                cancellationToken: _cardReadCts.Token);

            if (!result.IsSuccess)
                return;

            CardInfo = result.Card!.Uid;
            Steps[3].CompleteStepCommand.Execute(null);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    

    /// <summary>
    /// Печать чека о продаже.
    /// </summary>
    /// <param name="selling">Продажа.</param>
    private async Task PrintReceiptAsync(Selling selling)
    {
        var receipt = _receiptMappingService.MapSellingToSalesReceipt(selling);
        
        var printResult = await _receiptPrintService.PrintSalesReceiptAsync(receipt);
        
        _logger.LogInformation($"Чек отбит.\n Результаты печати: {printResult.Status}, {printResult.ErrorMessage}");
    }

    /// <summary>
    /// Вывести сообщение.
    /// </summary>
    /// <param name="title">Заголовок.</param>
    /// <param name="text">Текст сообщения.</param>
    private async Task ShowMessage(string title, string text)
    {
        _logger.LogInformation($"{title}: {text}");
        
        await MessageBoxManager
            .GetMessageBoxStandard(title, text)
            .ShowAsync();
    }
}