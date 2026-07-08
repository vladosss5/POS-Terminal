using Terminal.Application.Interfaces.Services;
using Terminal.Core.Models;

namespace Terminal.Application.Implementations.Services;

/// <inheritdoc/>
public class StatusNotifierService : IStatusNotifierService
{
    /// <summary>
    /// Коллекция статусов.
    /// </summary>
    private List<Status> StatusList { get; set; } = [];

    /// <summary>
    /// Коллекция подписчиков.
    /// </summary>
    private readonly List<IStatusObserver> _observers = [];
    
    /// <inheritdoc/>
    public void Attach(IStatusObserver observer)
    {
        _observers.Add(observer);

        if (StatusList.Count > 0)
            Notify();
    }

    /// <inheritdoc/>
    public void Detach(IStatusObserver observer)
    {
        _observers.Remove(observer);
    }

    /// <inheritdoc/>
    public void Notify()
    {
        foreach (var observer in _observers)
        {
            observer.UpdateStatuses(StatusList);
        }
    }

    /// <inheritdoc/>
    public void AddOrChangeStatus(Status status)
    {
        var existingStatus = StatusList.FirstOrDefault(x => x.Type == status.Type);

        if (existingStatus == null)
        {
            StatusList.Add(status);
            return;
        }

        existingStatus.IconName = status.IconName;
    }

    /// <inheritdoc/>
    public void RemoveStatus(Status status)
    {
        var existingStatus = StatusList.FirstOrDefault(x => x.Type == status.Type);

        if (existingStatus == null)
            return;
        
        StatusList.Remove(existingStatus);
    }

    /// <inheritdoc/>
    public void ChangeStatus(Status status)
    {
        var changingStatus = StatusList.FirstOrDefault(x => x.Type == status.Type);
        
        if (changingStatus == null)
            return;
        
        changingStatus.IconName = status.IconName;
    }
}