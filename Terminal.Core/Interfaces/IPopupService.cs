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

    /// <summary>
    /// Показать уведомление информирования.
    /// </summary>
    /// <param name="message">Текст сообщения.</param>
    public void ShowInfo(string message);

    /// <summary>
    /// Показать сообщение об ошибке.
    /// </summary>
    /// <param name="message">Текст сообщения.</param>
    public void ShowError(string message);

    /// <summary>
    /// Показать сообщение об успехе.
    /// </summary>
    /// <param name="message">Текст сообщения.</param>
    public void ShowSuccess(string message);
}