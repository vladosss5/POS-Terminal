using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Terminal.Application.Interfaces.Builders;
using Terminal.Core.Enums;
using Terminal.Core.Models;
using Terminal.ViewModels.Steps;

namespace Terminal.ViewModels.Pages;

public partial class RefuelingByCardPageViewModel : PageViewModelBase
{
    private readonly IRefuelingProcessBuilder _builder;
    
    [ObservableProperty] private ObservableCollection<StepViewModelBase> _steps;

    [ObservableProperty] private int _currentStepIndex;
    
    [ObservableProperty] private bool _isProcessStarted;
    
    [ObservableProperty] private PaymentTypes? _selectedCardType;
    
    [ObservableProperty] private string _selectedFuelType;
    
    [ObservableProperty] private decimal _amount;
    
    [ObservableProperty] private ObservableCollection<Refill> _completedProcesses;
    
    public IEnumerable<PaymentTypes> PaymentTypes => Enum.GetValues<PaymentTypes>();
    public IEnumerable<FuelTypes> FuelTypes => Enum.GetValues<FuelTypes>();


    public RefuelingByCardPageViewModel(IRefuelingProcessBuilder builder)
    {
        _builder = builder;
        CompletedProcesses = new();

        InitializeSteps();
        
        IsProcessStarted = true;
        CurrentStepIndex = 0;
        Steps[0].IsActive = true;
    }


    public void SetPaymentType(PaymentTypes type)
    {
        _builder.SetPaymentType(type);

        SelectedCardType = type;
        Steps[0].CompleteStepCommand.Execute(null);
    }

    public void SetFuelType(string type)
    {
        _builder.SetFuelType(type);

        SelectedFuelType = type;
        Steps[1].CompleteStepCommand.Execute(null);
    }
    
    public void SetCount(decimal count)
    {
        _builder.SetAmount(count);

        Amount = count;
        Steps[2].CompleteStepCommand.Execute(null);
    }

    
    public void StepBack()
    {
        
    }
    

    private void InitializeSteps()
    {
        Steps = new ObservableCollection<StepViewModelBase>
        {
            new("Тип оплаты", OnStepCompleted),
            new("Тип топлива", OnStepCompleted),
            new("Количество", OnStepCompleted)
        };
    }

    private void OnStepCompleted()
    {
        if (CurrentStepIndex < Steps.Count - 1)
        {
            CurrentStepIndex++;
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