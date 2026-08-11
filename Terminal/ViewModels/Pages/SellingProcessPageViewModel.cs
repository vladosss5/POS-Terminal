using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaEdit.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Enums;
using Terminal.Core.Interfaces;
using Terminal.Dtos;
using Terminal.Services.Mappers.ResourceCodeMapping;
using Terminal.ViewModels.Items;

namespace Terminal.ViewModels.Pages;

/// <summary>
/// Логика работы страницы продажи.
/// </summary>
public partial class SellingProcessPageViewModel : PageViewModelBase, IStepObserver
{
    /// <inheritdoc cref="ISalesProcessService" />
    private readonly ISalesProcessService _salesProcessService;

    /// <inheritdoc cref="IResourceCodeMapper" />
    private readonly IResourceCodeMapper _resourceCodeMapper;

    /// <inheritdoc cref="IStepNotifierService"/>
    private readonly IStepNotifierService _stepNotifierService;

    /// <inheritdoc cref="IPopupService"/>
    private readonly IPopupService _popupService;

    /// <summary>
    /// Культура для приведения чисел с точкой к строке.
    /// </summary>
    private readonly CultureInfo _culture = CultureInfo.InvariantCulture;

    /// <summary>
    /// Словарь для маппинга шага продажи к отображаемому элементу.
    /// </summary>
    private readonly Dictionary<SaleProcessStep, SellingStepViewModel> _stepMap;
    
    /// <summary>
    /// Типы оплаты.
    /// </summary>
    [ObservableProperty]
    public partial Dictionary<string, (BasePaymentType BaseType, DerivedPaymentType DerivedType)> PaymentTypesDictionary { get; set; }

    /// <summary>
    /// Коллекция цифровых кнопок. 
    /// </summary>
    public string[] KeypadButtons { get; } =
    [
        "7", "8", "9",
        "4", "5", "6",
        "1", "2", "3",
        "00", "0", "."
    ];

    /// <summary>
    /// Сообщения о типах кол-ва.
    /// </summary>
    public string[] AmountMessages { get; } =
    [
        "Указывается кол-во в ₽",
        "Указывается кол-во в литрах"
    ];

    /// <summary>
    /// Коллекция шагов заправки.
    /// </summary>
    public ObservableCollection<SellingStepViewModel> Steps { get; } = [];

    /// <summary>
    /// Св-во для хранения товаров (типов топлива).
    /// </summary>
    public ObservableCollection<ResourceCodeDto> Resources { get; set; } = [];

    /// <summary>
    /// Наименование текущей страницы (шага).
    /// </summary>
    [ObservableProperty]
    public partial string? NameCurrentPage { get; set; }

    /// <summary>
    /// Кол-во указано в деньгах?
    /// Если нет, то в единицах товара.
    /// </summary>
    [ObservableProperty]
    public partial bool IsAmountMoney { get; set; } = true;

    /// <summary>
    /// Кол-во в виде строки для отображения.
    /// </summary>
    [ObservableProperty] 
    public partial string AmountPreview { get; set; } = "0";
    
    /// <summary>
    /// Пароль пустой?
    /// </summary>
    [ObservableProperty] 
    public partial bool PinIsEmpty { get; private set; } = true;
    
    /// <summary>
    /// Предпросмотр пароля.
    /// </summary>
    [ObservableProperty] 
    public partial string? PinChar { get; private set; }

    /// <summary>
    /// Пароль в исходном виде.
    /// </summary>
    private string Pin
    {
        get;
        set
        {
            if (!SetProperty(ref field, value))
                return;

            PinChar = new string('*', value.Length);
            PinIsEmpty = string.IsNullOrEmpty(field);
        }
    } = "";
    
    /// <summary>
    /// Выбранный товар (тип топлива).
    /// </summary>
    private ResourceCodeDto? SelectedResourceCode { get; set; }


    /// <summary>
    /// Конструктор.
    /// </summary>
    public SellingProcessPageViewModel(
        ILogger<PageViewModelBase> logger,
        IStepNotifierService stepNotifierService,
        ISalesProcessService salesProcessService,
        IResourceCodeMapper resourceCodeMapper, 
        IPopupService popupService)
        : base(logger)
    {
        _stepNotifierService = stepNotifierService;
        _salesProcessService = salesProcessService;
        _resourceCodeMapper = resourceCodeMapper;
        _popupService = popupService;

        _ = LoadDataAsync();
        InitStepsCollection();

        _stepMap = Steps.ToDictionary(s => s.Step, s => s);
        PaymentTypesDictionary = _salesProcessService.GetAvailablePaymentTypes();
        
        stepNotifierService.Attach(this);
        var currentStep = stepNotifierService.GetCurrentStep();
        ChangeCurrentStep(currentStep);
    }


    /// <summary>
    /// Сделать шаг назад или выйти на предыдущий экран.
    /// </summary>
    [RelayCommand]
    private void StepBack()
    {
        if (_stepNotifierService.GetCurrentStep() == SaleProcessStep.SelectionResourceCode)
        {
            Navigation!.GoBack();
            return;
        }
        
        _stepNotifierService.StepBack();
    }

    /// <summary>
    /// Задать требуемый ресурс.
    /// </summary>
    /// <param name="resourceCodeDto">Dto ресурса.</param>
    [RelayCommand]
    private void SetFuelType(ResourceCodeDto resourceCodeDto)
    {
        var resourceCode = _resourceCodeMapper.MapResourceCodeDtoToDomainModel(resourceCodeDto);
        _salesProcessService.AddToCart(resourceCode);
        SelectedResourceCode = resourceCodeDto;
    }

    /// <summary>
    /// Задать товару запрашиваемое кол-во.
    /// </summary>
    [RelayCommand]
    private void SetAmount()
    {
        if (SelectedResourceCode == null ||
            SelectedResourceCode.ResourcePrice == 0)
        {
            _popupService.ShowInfo("Задайте кол-во отличное от 0");
            return;
        }
        
        var amount = decimal.Parse(AmountPreview, _culture);
        var calculatedField = IsAmountMoney ? CalculatedField.Amount : CalculatedField.Price;

        _salesProcessService.SetAmount(SelectedResourceCode.ResourceKey, amount, calculatedField);
    }

    /// <summary>
    /// Задать продаже тип оплаты.
    /// </summary>
    /// <param name="typeKey">Ключ типа.</param>
    [RelayCommand]
    private async Task SetPaymentTypeAsync(string typeKey)
    {
        try
        {
            if (!PaymentTypesDictionary.TryGetValue(typeKey, out var value))
                return;

            _salesProcessService.SetPaymentType(value.BaseType, value.DerivedType);

            if (value.DerivedType is DerivedPaymentType.BankCard or DerivedPaymentType.FuelCard)
                await _salesProcessService.ReadCardAsync();

            await _salesProcessService.CompleteProcessAsync();
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message, e.InnerException);
            _popupService.ShowError(e.Message);
        }
        finally
        {
            Navigation!.NavigateTo<MainMenuPageViewModel>();
        }
    }

    /// <summary>
    /// Сменить режим ввода запрашиваемого кол-ва.
    /// </summary>
    [RelayCommand]
    private void ToggleMode()
    {
        if (SelectedResourceCode != null &&
            SelectedResourceCode.ResourcePrice != 0)
        {
            var amount = decimal.Parse(AmountPreview, _culture);
            amount = Math.Round(amount, IsAmountMoney ? 2 : 3);
            
            if (amount != 0)
            {
                var newValue = IsAmountMoney
                    ? amount / SelectedResourceCode.ResourcePrice
                    : amount * SelectedResourceCode.ResourcePrice;

                newValue = Math.Round(newValue, IsAmountMoney ? 3 : 2);
                AmountPreview = newValue.ToString(_culture);
            }
        }
        
        IsAmountMoney = !IsAmountMoney;
    }
    
    /// <summary>
    /// Добавить цифру в конец кол-ва.
    /// </summary>
    /// <param name="number">Символ цифры.</param>
    [RelayCommand]
    private void AddNumber(string number)
    {
        var maxDecimals = IsAmountMoney ? 2 : 3;
        var dotIndex = AmountPreview.IndexOf('.');
        
        if (AmountPreview == "0")
            AmountPreview = string.Empty;
        
        if (AmountPreview.Length == 26)
            return;
        
        if (number == "." && dotIndex != -1)
            return;

        if (AmountPreview.Length - dotIndex - 1 >= maxDecimals && dotIndex != -1)
            return;
        
        AmountPreview += number;
    }
    
    /// <summary>
    /// Удалить последнюю цифру из кол-ва. 
    /// </summary>
    public void RemoveLastNumber() => AmountPreview = AmountPreview.Length > 1 ? AmountPreview[..^1] : "0";

    /// <summary>
    /// Стереть кол-во.
    /// </summary>
    public void SetZero() => AmountPreview = "0";
    
    /// <summary>
    /// Добавить символ к паролю.
    /// </summary>
    /// <param name="element">Символ.</param>
    [RelayCommand]
    private void AddCharInPin(string element) => Pin += element;
    
    /// <summary>
    /// Удалить последний символ из пароля.
    /// </summary>
    public void RemoveLastCharFromPin() => Pin = Pin[..^1];

    /// <summary>
    /// Ввести пин.
    /// </summary>
    [RelayCommand]
    private void EnterPin()
    {
        if (string.IsNullOrEmpty(Pin))
        {
            _popupService.ShowError("Введите пароль!");
            return;
        }

        _salesProcessService.EnterPin(Pin);
    }
    
    /// <inheritdoc/>
    public void ChangeCurrentStep(SaleProcessStep step)
    {
        if (!_stepMap.TryGetValue(step, out var activeStep))
            return;
        
        activeStep.Activate(Steps);
        NameCurrentPage = activeStep.Name;
    }

    /// <summary>
    /// Инициализация шагов.
    /// </summary>
    private void InitStepsCollection()
    {
        Steps.AddRange(
        [
            new SellingStepViewModel(SaleProcessStep.SelectionResourceCode, "Товар"),
            new SellingStepViewModel(SaleProcessStep.SettingAmount, "Количество"),
            new SellingStepViewModel(SaleProcessStep.SelectionPaymentType, "Тип оплаты"),
            new SellingStepViewModel(SaleProcessStep.CardReading, "Считывание"),
            new SellingStepViewModel(SaleProcessStep.EnteringPin, "Пин карты")
        ]);
    }
    
    /// <summary>
    /// Подгрузка данных из БД.
    /// </summary>
    private async Task LoadDataAsync()
    {
        try
        {
            var resources = await _salesProcessService.GetAvailableResourceCodesAsync();
            var dtoResources = _resourceCodeMapper.MapResourceCodeDomainModelToDtoRange(resources);
        
            Resources = [.. dtoResources];
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message, e.InnerException);
            _popupService.ShowError($"Данные не загружены из-за ошибки: {e.Message}");
        }
    }
}