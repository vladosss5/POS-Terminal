using Terminal.Core.Enums;

namespace Terminal.Application.Interfaces.Services;

/// <summary>
/// Контракт наблюдателя за шагами продажи.
/// </summary>
public interface IStepObserver
{
    /// <summary>
    /// Сменить текущий шаг.
    /// </summary>
    /// <param name="step">Шаг продажи.</param>
    public void ChangeCurrentStep(SaleProcessStep step);
}