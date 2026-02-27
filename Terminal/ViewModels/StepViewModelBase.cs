using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Terminal.ViewModels;

/// <summary>
/// Модель шага процесса.
/// </summary>
public partial class StepViewModelBase : ViewModelBase
{
    /// <summary>
    /// Отметить шаг выполненным.
    /// </summary>
    private readonly Func<Task> _onStepCompleted;

    /// <summary>
    /// Наименование шага.
    /// </summary>
    [ObservableProperty] private string _stepName;
    
    /// <summary>
    /// Шаг выполнен?
    /// </summary>
    [ObservableProperty] private bool _isCompleted;
    
    /// <summary>
    /// Шаг активен?
    /// </summary>
    [ObservableProperty] private bool _isActive;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="stepName">Наименование шага.</param>
    /// <param name="onStepCompleted">Метод помечания шага выполненным.</param>
    public StepViewModelBase(string stepName, 
        Func<Task> onStepCompleted)
    {
        _stepName = stepName;
        _onStepCompleted = onStepCompleted;
    }
    
    /// <summary>
    /// Базовый метод отом что метод выполнился.
    /// </summary>
    [RelayCommand]
    private async Task CompleteStep()
    {
        if (!IsActive) return;
        
        IsCompleted = true;
        IsActive = false;
        _onStepCompleted?.Invoke();
    }
}