using Terminal.Core.Entities.Models;
using Terminal.Core.Enums;
using Terminal.Core.Interfaces;

namespace Terminal.Application.Services;

/// <inheritdoc/>
public class PopupService : IPopupService
{
    /// <summary>
    /// Список наблюдателей.
    /// </summary>
    private readonly List<IPopupObserver> _observers = [];
    
    /// <summary>
    /// Список уведомлений.
    /// </summary>
    private readonly List<Popup> _popups = [];
    
    /// <inheritdoc/>
    public void Attach(IPopupObserver observer)
    {
        _observers.Add(observer);
    }

    /// <inheritdoc/>
    public void Detach(IPopupObserver observer)
    {
        _observers.Remove(observer);
    }

    /// <inheritdoc/>
    public void ShowCustomPopup(Popup popup)
    {
        _popups.Add(popup);
        Notify();

        _ = Task.Run(async () =>
        {
            await Task.Delay(popup.DurationMs);

            _popups.Remove(popup);
            Notify();
        });
    }

    /// <inheritdoc/>
    public void ShowInfo(string message)
    {
        ShowCustomPopup(new Popup(message, PopupType.Info));
    }

    /// <inheritdoc/>
    public void ShowError(string message)
    {
        ShowCustomPopup(new Popup(message, PopupType.Error, 10000));
    }

    /// <inheritdoc/>
    public void ShowSuccess(string message)
    {
        ShowCustomPopup(new Popup(message, PopupType.Success, 3000));
    }

    /// <summary>
    /// Оповестить наблюдателей об изменениях.
    /// </summary>
    private void Notify()
    {
        foreach (var observer in _observers)
            observer.OnPopupChanged(_popups);
    }
}