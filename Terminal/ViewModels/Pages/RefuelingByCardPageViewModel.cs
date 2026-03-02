using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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

namespace Terminal.ViewModels.Pages;

/// <summary>
/// Страница процесса заправки по карте.
/// </summary>
public partial class RefuelingByCardPageViewModel : PageViewModelBase
{
    /// Фабрика создающая <inheritdoc cref="DataContext"/>
    private readonly IDbContextFactory<DataContext> _dbFactory;

    private readonly IPrintService _printService;
    
    /// <inheritdoc cref="ISellingBuilder"/>
    private readonly ISellingBuilder _builder;
    
    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger<RefuelingByCardPageViewModel> _logger;

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
    /// Кол-во без указания ед.изм.
    /// </summary>
    [ObservableProperty] private decimal _amount;

    /// <summary>
    /// Превьювер кол-ва.
    /// </summary>
    [ObservableProperty] private string _amountPreview = "0";

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
    /// Св-во для хранения типов оплаты.
    /// </summary>
    public IEnumerable<PaymentTypes> PaymentTypes => Enum.GetValues<PaymentTypes>();

    /// <summary>
    /// Св-во для хранения товаров (типов топлива).
    /// </summary>
    [ObservableProperty] private ObservableCollection<ResourceCode> _resources;

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
    /// <param name="count">Кол-во без ед. изм.</param>
    public async void SetCount(decimal count)
    {
        _builder.SetAmount(count);

        Amount = count;

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
        if (AmountPreview == "0")
            AmountPreview = string.Empty;

        if (AmountPreview.Length > 10)
            return;
        
        AmountPreview += item;
    }

    /// <summary>
    /// Удалить последний символ из превьювера кол-ва.
    /// </summary>
    public void DeleteLastChar()
    {
        AmountPreview = AmountPreview[..^1];
    }

    /// <summary>
    /// Сменить единицу измерения (деньги на литры).
    /// </summary>
    public void SwitchAmount()
    {
        if (IsAmountMoney)
        {
            IsAmountMoney = false;
            AmountWhat = _amountMessages[1];
        }
        else
        {
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