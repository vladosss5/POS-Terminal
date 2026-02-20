using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Terminal.ViewModels.Steps;

public partial class StepViewModelBase : ViewModelBase
{
    private readonly Action _onStepCompleted;

    [ObservableProperty] private string _stepName;
    [ObservableProperty] private bool _isCompleted;
    [ObservableProperty] private bool _isActive;

    public StepViewModelBase(string stepName, Action onStepCompleted)
    {
        _stepName = stepName;
        _onStepCompleted = onStepCompleted;
    }
    
    [RelayCommand]
    private void CompleteStep()
    {
        if (!IsActive) return;
        
        IsCompleted = true;
        IsActive = false;
        _onStepCompleted?.Invoke();
    }
}