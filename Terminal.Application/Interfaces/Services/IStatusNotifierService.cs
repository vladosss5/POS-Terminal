using Terminal.Core.Entities.Models;

namespace Terminal.Application.Interfaces.Services;

/// <summary>
/// Сервис оповещения о смене статуса приложения.
/// </summary>
public interface IStatusNotifierService
{
    /// <summary>
    /// Подписаться на события.
    /// </summary>
    /// <param name="observer">Наблюдатель за статусом.</param>
    public void Attach(IStatusObserver observer);

    /// <summary>
    /// Отписаться от событий.
    /// </summary>
    /// <param name="observer">Наблюдатель за статусом.</param>
    public void Detach(IStatusObserver observer);

    /// <summary>
    /// Оповестить об изменении.
    /// </summary>
    public void Notify();

    /// <summary>
    /// Добавить статус в коллекцию.
    /// </summary>
    /// <param name="status">Статус.</param>
    public void AddOrChangeStatus(Status status);
    
    /// <summary>
    /// Удалить статус из коллекции.
    /// </summary>
    /// <param name="status">Статус.</param>
    public void RemoveStatus(Status status);
    
    /// <summary>
    /// Сменить статус.
    /// </summary>
    /// <param name="status">Статус.</param>
    public void ChangeStatus(Status status);
}