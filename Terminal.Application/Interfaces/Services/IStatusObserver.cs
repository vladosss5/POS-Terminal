using Terminal.Core.Models;

namespace Terminal.Application.Interfaces.Services;

/// <summary>
/// Контракт о том что объект является наблюдателем за статусами приложения.
/// </summary>
public interface IStatusObserver
{
    /// <summary>
    /// Метод вызываемый в наблюдателе при обновлении статусов в сервисе.
    /// </summary>
    /// <param name="statusList">Коллекция статусов.</param>
    public void UpdateStatuses(List<Status> statusList);
}