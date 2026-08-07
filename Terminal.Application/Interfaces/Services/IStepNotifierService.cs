using Terminal.Core.Enums;

namespace Terminal.Application.Interfaces.Services;

/// <summary>
/// Машина состояний шагов продажи.
/// </summary>
public interface IStepNotifierService
{
    /// <summary>
    /// Подписаться на события.
    /// </summary>
    /// <param name="observer">Наблюдатель за статусом.</param>
    public void Attach(IStepObserver observer);

    /// <summary>
    /// Отписаться от событий.
    /// </summary>
    /// <param name="observer">Наблюдатель за статусом.</param>
    public void Detach(IStepObserver observer);

    /// <summary>
    /// Оповестить об изменении.
    /// </summary>
    public void Notify();

    /// <summary>
    /// Выполнить текущий шаг.
    /// </summary>
    public void CompleteCurrentStep();

    /// <summary>
    /// Перейти к указанному шагу.
    /// </summary>
    /// <param name="step">Шаг.</param>
    public void GoToStep(SaleProcessStep step);

    /// <summary>
    /// Вернуться к предыдущему шагу.
    /// </summary>
    public void StepBack();

    /// <summary>
    /// Получить текущий шаг.
    /// </summary>
    /// <returns>Тип шага.</returns>
    public SaleProcessStep GetCurrentStep();
}