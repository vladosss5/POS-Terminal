using Terminal.Application.Interfaces.Services;
using Terminal.Core.Enums;

namespace Terminal.Application.Services;

/// <inheritdoc/>
public class StepNotifierService : IStepNotifierService
{
    /// <summary>
    /// Порядок шагов продажи.
    /// </summary>
    private static readonly SaleProcessStep[] Steps = 
    [
        SaleProcessStep.SelectionResourceCode,
        SaleProcessStep.SettingAmount,
        SaleProcessStep.SelectionPaymentType,
        SaleProcessStep.CardReading,
        SaleProcessStep.Discounting,
        SaleProcessStep.Debit,
        SaleProcessStep.SaveToDataBase,
        SaleProcessStep.PrintReceipt
    ];
    
    /// <summary>
    /// Текущий шаг.
    /// </summary>
    private SaleProcessStep _currentStep = Steps[0];
    
    /// <summary>
    /// Список подписчиков.
    /// </summary>
    private readonly List<IStepObserver> _observers = [];

    /// <inheritdoc/>
    public void Attach(IStepObserver observer)
    {
        _observers.Add(observer);
    }

    /// <inheritdoc/>
    public void Detach(IStepObserver observer)
    {
        _observers.Remove(observer);
    }

    /// <inheritdoc/>
    public void Notify()
    {
        foreach (var observer in _observers)
        {
            observer.ChangeCurrentStep(_currentStep);
        }
    }

    /// <inheritdoc/>
    public void CompleteCurrentStep()
    {
        var currentIndex = Array.IndexOf(Steps, _currentStep);
        _currentStep = Steps[++currentIndex];

        Notify();
    }

    /// <inheritdoc/>
    public void StepBack()
    {
        var currentIndex = Array.IndexOf(Steps, _currentStep);
        _currentStep = Steps[--currentIndex];
        
        Notify();
    }

    /// <inheritdoc/>
    public void RetryCurrentStep()
    {
        Notify();
    }

    public SaleProcessStep GetCurrentStep()
    {
        return _currentStep;
    }
}