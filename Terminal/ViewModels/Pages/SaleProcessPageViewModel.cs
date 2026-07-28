using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MsBox.Avalonia;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Entities.DbEntities.MainDb;
using Terminal.Core.Enums;
using Terminal.Core.Interfaces;
using Terminal.ViewModels.Items;

namespace Terminal.ViewModels.Pages;

/// <summary>
/// Страница процесса заправки по карте.
/// </summary>
public partial class SaleProcessPageViewModel : PageViewModelBase
{
    /// <inheritdoc cref="ILogger"/>
    private readonly ILogger<SaleProcessPageViewModel> _logger;

    /// <inheritdoc cref="ICardReaderService" />
    private readonly ICardReaderService _cardReaderService;
    
    /// <inheritdoc cref="ISalesProcessService" />
    private readonly ISalesProcessService _salesProcessService;

    
    /// <summary>
    /// Культура для приведения чисел с точкой к строке.
    /// </summary>
    private readonly CultureInfo _culture = CultureInfo.InvariantCulture;
    
    /// <summary>
    /// Кол-во топлива.
    /// </summary>
    private decimal _amountFuel;

    /// <summary>
    /// Сообщения о типах кол-ва.
    /// </summary>
    private readonly string[] _amountMessages =
    [
        "Указывается кол-во в ₽",
        "Указывается кол-во в литрах"
    ];
    
    /// <summary>
    /// Токен отмены считывания карты.
    /// </summary>
    private CancellationTokenSource? _cardReadCts;

    /// <summary>
    /// Индекс текущего шага.
    /// </summary>
    [ObservableProperty] 
    public partial int CurrentStepIndex { get; set; }

    /// <summary>
    /// Процесс начат?
    /// </summary>
    [ObservableProperty]
    public partial bool IsProcessStarted { get; set; }

    /// <summary>
    /// Выбранный тип топлива (товар).
    /// </summary>
    [ObservableProperty]
    public partial ResourceCode? SelectedResourceCode { get; set; }

    /// <summary>
    /// Кол-во указано в деньгах?
    /// Если нет, то в литрах.
    /// </summary>
    [ObservableProperty]
    public partial bool IsAmountMoney { get; set; } = true;

    /// <summary>
    /// Сообщение-указатель на единицу измерения для пользователя.
    /// </summary>
    [ObservableProperty]
    public partial string AmountWhat { get; set; }

    /// <summary>
    /// Наименование текущей страницы (шага).
    /// </summary>
    [ObservableProperty]
    public partial string NameCurrentPage { get; set; }

    /// <summary>
    /// Св-во для хранения товаров (типов топлива).
    /// </summary>
    [ObservableProperty]
    public partial ObservableCollection<ResourceCode> Resources { get; set; }

    /// <summary>
    /// Коллекция шагов заправки.
    /// </summary>
    [ObservableProperty]
    public partial ObservableCollection<StepViewModelBase> Steps { get; private set; }

    /// <summary>
    /// Типы оплаты.
    /// </summary>
    [ObservableProperty]
    public partial Dictionary<string, (BasePaymentType BaseType, DerivedPaymentType DerivedType)> PaymentTypesDictionary { get; set; }

    /// <summary>
    /// Коллекция цифровых кнопок. 
    /// </summary>
    public ObservableCollection<string> KeypadButtons { get; } =
    [
        "7", "8", "9",
        "4", "5", "6",
        "1", "2", "3",
        "00", "0", "."
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
            
            if (decimal.TryParse(value, NumberStyles.Any, _culture, out var d))
                _amountFuel = d / (SelectedResourceCode?.ResourcePrice ?? 1m);
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
            
            if (decimal.TryParse(value, NumberStyles.Any, _culture, out var d))
                _amountFuel = d;
        }
    }

    /// <summary>
    /// Конструктор.
    /// </summary>
    public SaleProcessPageViewModel(
        ILogger<SaleProcessPageViewModel> logger, 
        ICardReaderService cardReaderService, 
        ISalesProcessService salesProcessService) 
        : base(logger)
    {
        _logger = logger;
        _cardReaderService = cardReaderService;
        _salesProcessService = salesProcessService;

        InitializeSteps();
        _ = LoadDataAsync();
        
        AmountWhat = IsAmountMoney ? _amountMessages[0] : _amountMessages[1];
        PaymentTypesDictionary = _salesProcessService.GetAvailablePaymentTypes();
    }


    /// <summary>
    /// Указать тип оплаты.
    /// </summary>
    /// <param name="typeKey">Тип оплаты.</param>
    [RelayCommand]
    private async Task SetPaymentType(string typeKey)
    {
        if (!PaymentTypesDictionary.TryGetValue(typeKey, out var value)) 
            return;

        await _salesProcessService.SetPaymentTypeAsync(value.BaseType, value.DerivedType);
        
        if (value.DerivedType is DerivedPaymentType.BankCard or DerivedPaymentType.FuelCard)
        {
            try
            {
                Steps[2].CompleteStepCommand.Execute(null);
                await ProcessCardForPaymentAsync();
            }
            catch(Exception e)
            {
                _logger.LogError($"{e.Message}, {e.InnerException}");
            }
        }
        else
        {
            await CompleteRefuelingProcess();
        }
    }

    /// <summary>
    /// Указать тип топлива (товара).
    /// </summary>
    /// <param name="resource">Топливо.</param>
    [RelayCommand]
    private async Task SetFuelType(ResourceCode resource)
    {
        await _salesProcessService.AddToCartAsync(resource);

        SelectedResourceCode = resource;
        Steps[0].CompleteStepCommand.Execute(null);
    }
    
    /// <summary>
    /// Указать кол-во топлива.
    /// </summary>
    public void SetAmount()
    {
        _salesProcessService.SetAmount(SelectedResourceCode!.ResourceKey, _amountFuel, IsAmountMoney);
        Steps[1].CompleteStepCommand.Execute(null);
    }
    
    /// <summary>
    /// Добавить символы в предпросмотр кол-ва.
    /// </summary>
    /// <param name="symbols">Символ.</param>
    [RelayCommand]
    private void AddCharInAmountPreview(string symbols)
    {
        foreach (var symbol in symbols)
        {
            var current = IsAmountMoney ? AmountMoneyPreview : AmountFuelPreview;
            var maxDecimals = IsAmountMoney ? 2 : 3;

            var dotIndex = current.IndexOf('.');
        
            if (dotIndex >= 0 && symbol != '.')
            {
                var decimalsAfterDot = current.Length - dotIndex - 1;
            
                if (decimalsAfterDot >= maxDecimals) 
                    return;
            }
        
            if (symbol == '.' && current.Contains(symbol))
                return;

            if (current == "0" && symbol == '.')
                current = "0";

            string newValue;

            if (current == "0" && symbol != '.')
                newValue = symbol.ToString(_culture);
            else
                newValue = current + symbol;
        
            if (newValue.Length > 14) 
                return;

            if (IsAmountMoney)
                AmountMoneyPreview = newValue;
            else
                AmountFuelPreview = newValue;
        }
    }

    /// <summary>
    /// Удалить последний символ из предпросмотра кол-ва.
    /// </summary>
    public void DeleteLastCharFromPreview()
    {
        if (IsAmountMoney)
            AmountMoneyPreview = AmountMoneyPreview.Length > 1 ? AmountMoneyPreview[..^1] : "0";
        else
            AmountFuelPreview = AmountFuelPreview.Length > 1 ? AmountFuelPreview[..^1] : "0";
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
            if (!decimal.TryParse(AmountMoneyPreview, NumberStyles.Any, _culture, out var money))
                return;
            
            _amountFuel = money / (SelectedResourceCode?.ResourcePrice ?? 1m);
            AmountFuelPreview = _amountFuel
                .ToString($"N3", _culture)
                .TrimEnd('0')
                .TrimEnd('.');
                
            IsAmountMoney = false;
            AmountWhat = _amountMessages[1];
        }
        else
        {
            if (!decimal.TryParse(AmountFuelPreview, NumberStyles.Any, _culture, out var fuel)) 
                return;
            
            AmountMoneyPreview = (fuel * (SelectedResourceCode?.ResourcePrice ?? 1m))
                .ToString($"N2", _culture)
                .TrimEnd('0')
                .TrimEnd('.');
                
            IsAmountMoney = true;
            AmountWhat = _amountMessages[0];
        }
    }

    /// <summary>
    /// Пометить шаг выполненным.
    /// </summary>
    private async void OnStepCompleted()
    {
        try
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
        catch (Exception e)
        {
            _logger.LogError(e.Message, e.InnerException);
        }
    }
    
    /// <summary>
    /// Завершить процесс заправки по карте.
    /// </summary>
    private async Task CompleteRefuelingProcess()
    {
        try
        {
            await _salesProcessService.CompleteProcessAsync();
            Navigation!.GoBack();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка: {ex.Message},\n {ex.InnerException}, \n {ex.StackTrace}" );
            await ShowMessage("Ошибка!", $"{ex.Message}, {ex.InnerException}");
        }
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

            await _salesProcessService.CalculateDiscountAsync(result.Card!);

            Steps[3].CompleteStepCommand.Execute(null);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
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
            Navigation!.GoBack();
        }
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
        IsProcessStarted = true;
        CurrentStepIndex = 0;
        Steps[0].IsActive = true;
    }

    /// <summary>
    /// Подгрузка данных из БД.
    /// </summary>
    private async Task LoadDataAsync()
    {
        var resources = await _salesProcessService.GetAvailableResourceCodesAsync();
        Resources = new ObservableCollection<ResourceCode>(resources);
    }
}