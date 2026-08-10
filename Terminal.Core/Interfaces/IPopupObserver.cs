using Terminal.Core.Entities.Models;

namespace Terminal.Core.Interfaces;

/// <summary>
/// Контракт наблюдателя за всплывающими уведомлениями.
/// </summary>
public interface IPopupObserver
{
    /// <summary>
    /// Уведомления обновлены.
    /// </summary>
    /// <param name="popups">Коллекция уведомлений.</param>
    public void OnPopupChanged(List<Popup> popups);
}