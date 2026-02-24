using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using Terminal.Application.Interfaces.Builders;
using Terminal.Core.DbEntities;
using Terminal.Core.Enums;
using Terminal.Core.Models;
using Terminal.Data.Context;
using Terminal.ViewModels.Steps;

namespace Terminal.ViewModels.Pages;

public partial class RefuelingByCardPageViewModel : PageViewModelBase
{
    private readonly IDbContextFactory<DataContext> _dbFactory;
    private readonly IRefuelingProcessBuilder _builder;

    private string[] _amountMessages = new []
    {
        "Указывается кол-во в ₽",
        "Указывается кол-во в литрах"
    };
    
    [ObservableProperty] private ObservableCollection<StepViewModelBase> _steps;

    [ObservableProperty] private int _currentStepIndex;
    
    [ObservableProperty] private bool _isProcessStarted;
    
    [ObservableProperty] private PaymentTypes? _selectedCardType;
    
    [ObservableProperty] private Product? _selectedFuelType;
    
    [ObservableProperty] private decimal _amount;

    [ObservableProperty] private string _amountPreview = "0";

    [ObservableProperty] private bool _isAmountMoney = true;

    [ObservableProperty] private string _amountWhat;
    
    [ObservableProperty] private ObservableCollection<Refill> _completedProcesses;

    [ObservableProperty] private string _nameCurrentPage;
    
    public IEnumerable<PaymentTypes> PaymentTypes => Enum.GetValues<PaymentTypes>();
    
    public List<Product> Products { get; set; }

    public RefuelingByCardPageViewModel(
        IRefuelingProcessBuilder builder, 
        IDbContextFactory<DataContext> dbFactory)
    {
        _builder = builder;
        _dbFactory = dbFactory;
        CompletedProcesses = new();

        InitializeSteps();
        _ = LoadDataAsync();
        
        IsProcessStarted = true;
        CurrentStepIndex = 0;
        Steps[0].IsActive = true;
        
        _amountWhat = IsAmountMoney ? _amountMessages[0] : _amountMessages[1];
    }


    public void SetPaymentType(PaymentTypes type)
    {
        _builder.SetPaymentType(type);

        SelectedCardType = type;
        Steps[0].CompleteStepCommand.Execute(null);
    }

    public void SetFuelType(Product type)
    {
        _builder.SetFuelType(type);

        SelectedFuelType = type;
        Steps[1].CompleteStepCommand.Execute(null);
    }
    
    public async void SetCount(decimal count)
    {
        _builder.SetAmount(count);

        Amount = count;

        if (true)
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Ещё не реализовано").ShowAsync();
            return;
        }
        Steps[2].CompleteStepCommand.Execute(null);
    }

    
    public void StepBack()
    {
        if (!IsProcessStarted || CurrentStepIndex <= 0)
            return;

        Steps[CurrentStepIndex].IsActive = false;
        
        CurrentStepIndex--;
        NameCurrentPage = Steps[CurrentStepIndex].StepName;
        
        var prevStep = Steps[CurrentStepIndex];
        prevStep.IsActive = true;
        prevStep.IsCompleted = false;
    }

    public void AddCharInAmountPreview(string item)
    {
        if (AmountPreview == "0")
            AmountPreview = string.Empty;

        if (AmountPreview.Length > 10)
            return;
        
        AmountPreview += item;
    }

    public void DeleteLastChar()
    {
        AmountPreview = AmountPreview[..^1];
    }

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

    private async Task LoadDataAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        
        var products = await db.Products
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ToListAsync();

        Products = products;
    }

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

    private void OnStepCompleted()
    {
        if (CurrentStepIndex < Steps.Count - 1)
        {
            CurrentStepIndex++;
            NameCurrentPage = Steps[CurrentStepIndex].StepName;
            Steps[CurrentStepIndex].IsActive = true;
        }
        else
        {
            CompleteRefuelingProcess();
        }
    }
    
    private void CompleteRefuelingProcess()
    {
        try
        {
            var process = _builder.Build();
            
            CompletedProcesses.Add(process);
            
            ResetProcess();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка: {ex.Message}");
        }
    }
    
    private void ResetProcess()
    {
        SelectedCardType = null;
        SelectedFuelType = null;
        Amount = 0;
        IsProcessStarted = false;
        CurrentStepIndex = 0;
        
        foreach (var step in Steps)
        {
            step.IsActive = false;
            step.IsCompleted = false;
        }
    }
    
    private bool IsStepActive(int stepIndex)
    {
        return IsProcessStarted && CurrentStepIndex == stepIndex && Steps[stepIndex].IsActive;
    }
}