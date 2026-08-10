using Terminal.Core.Entities.Models;
using Terminal.Core.Enums;

namespace Terminal.Core.Interfaces;

/// <summary>
/// Сервис всплывающих уведомлений. 
/// </summary>
public interface IPopupService
{
    /// <summary>
    /// Подписаться на обновления.
    /// </summary>
    /// <param name="observer">Наблюдатель за всплывающими сообщениями.</param>
    public void Attach(IPopupObserver observer);
    
    /// <summary>
    /// Отписаться от обновлений.
    /// </summary>
    /// <param name="observer">Наблюдатель за всплывающими сообщениями.</param>
    public void Detach(IPopupObserver observer);

    /// <summary>
    /// Показать индивидуальное уведомление.
    /// </summary>
    /// <param name="popup">Данные всплывающего уведомления.</param>
    public void ShowCustomPopup(Popup popup);
}