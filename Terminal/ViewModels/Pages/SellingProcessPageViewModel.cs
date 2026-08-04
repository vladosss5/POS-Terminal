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
using Terminal.Application.Services;
using Terminal.Core.Enums;
using Terminal.Dtos;
using Terminal.Services.Mappers.ResourceCodeMapping;
using Terminal.ViewModels.Items;

namespace Terminal.ViewModels.Pages;

public partial class SellingProcessPageViewModel : PageViewModelBase, IStepObserver
{
    /// <inheritdoc cref="ISalesProcessService" />
    private readonly ISalesProcessService _salesProcessService;

    /// <inheritdoc cref="IResourceCodeMapper" />
    private readonly IResourceCodeMapper _resourceCodeMapper;


    /// <summary>
    /// Культура для приведения чисел с точкой к строке.
    /// </summary>
    private readonly CultureInfo _culture = CultureInfo.InvariantCulture;

    /// <summary>
    /// Словарь для маппинга шага продажи к отображаемому элементу.
    /// </summary>
    private readonly Dictionary<SaleProcessStep, SellingStepViewModel> _stepMap;

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
    public partial string NameCurrentPage { get; set; }

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
        IResourceCodeMapper resourceCodeMapper)
        : base(logger)
    {
        _salesProcessService = salesProcessService;
        _resourceCodeMapper = resourceCodeMapper;

        _ = LoadDataAsync();
        InitStepsCollection();

        _stepMap = Steps.ToDictionary(s => s.Step, s => s);
        PaymentTypesDictionary = _salesProcessService.GetAvailablePaymentTypes();
        
        stepNotifierService.Attach(this);
        var currentStep = stepNotifierService.GetCurrentStep();
        ChangeCurrentStep(currentStep);
    }


    [RelayCommand]
    private void StepBack()
    {
        // Сброс текущих изменений.
        // Вызов сервиса для перехода на предыдущий шаг.
    }

    [RelayCommand]
    private async Task SetFuelType(ResourceCodeDto resourceCodeDto)
    {
        var resourceCode = _resourceCodeMapper.MapResourceCodeDtoToDomainModel(resourceCodeDto);
        _salesProcessService.AddToCart(resourceCode);
        SelectedResourceCode = resourceCodeDto;
    }

    [RelayCommand]
    private async Task SetAmount()
    {
        if (SelectedResourceCode == null || 
            SelectedResourceCode.ResourcePrice == 0)
            return;
        
        var amount = decimal.Parse(AmountPreview, _culture);
        var calculatedField = IsAmountMoney ? CalculatedField.Amount : CalculatedField.Price;

        _salesProcessService.SetAmount(SelectedResourceCode.ResourceKey, amount, calculatedField);
    }

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

    public void RemoveLastNumber() => AmountPreview = AmountPreview.Length > 1 ? AmountPreview[..^1] : "0";

    public void SetZero() => AmountPreview = "0";
    
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
            new SellingStepViewModel(SaleProcessStep.Discounting, "Пред. расчёт"),
            new SellingStepViewModel(SaleProcessStep.Debit, "Дебетование"),
            new SellingStepViewModel(SaleProcessStep.SaveToDataBase, "Сохранение"),
            new SellingStepViewModel(SaleProcessStep.PrintReceipt, "Печать чека")
        ]);
    }
    
    /// <summary>
    /// Подгрузка данных из БД.
    /// </summary>
    private async Task LoadDataAsync()
    {
        var resources = await _salesProcessService.GetAvailableResourceCodesAsync();
        var dtoResources = _resourceCodeMapper.MapResourceCodeDomainModelToDtoRange(resources);
        
        Resources = [.. dtoResources];
    }
}