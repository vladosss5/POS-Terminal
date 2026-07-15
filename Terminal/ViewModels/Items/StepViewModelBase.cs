using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Terminal.ViewModels.Items;

/// <summary>
/// Модель шага процесса.
/// </summary>
public partial class StepViewModelBase : ViewModelBase
{
    /// <summary>
    /// Отметить шаг выполненным.
    /// </summary>
    private readonly Action _onStepCompleted;

    /// <summary>
    /// Наименование шага.
    /// </summary>
    [ObservableProperty]
    public partial string StepName { get; set; }

    /// <summary>
    /// Шаг выполнен?
    /// </summary>
    [ObservableProperty]
    public partial bool IsCompleted { get; set; }

    /// <summary>
    /// Шаг активен?
    /// </summary>
    [ObservableProperty]
    public partial bool IsActive { get; set; }

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="stepName">Наименование шага.</param>
    /// <param name="onStepCompleted">Метод помечания шага выполненным.</param>
    public StepViewModelBase(string stepName, Action onStepCompleted)
    {
        StepName = stepName;
        _onStepCompleted = onStepCompleted;
    }
    
    /// <summary>
    /// Базовый метод отом что метод выполнился.
    /// </summary>
    [RelayCommand]
    private void CompleteStep()
    {
        if (!IsActive) return;
        
        IsCompleted = true;
        IsActive = false;
        _onStepCompleted?.Invoke();
    }
}