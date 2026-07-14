using System.Threading.Tasks;
using MsBox.Avalonia.Enums;

namespace Terminal.Services.MessageBoxService;

/// <summary>
/// Сервис вывода сообщений.
/// </summary>
public interface IMessageBoxService
{
    /// <summary>
    /// Показать сообщение.
    /// </summary>
    /// <param name="title">Заголовок окна сообщения.</param>
    /// <param name="message">Сообщение.</param>
    /// <param name="buttonEnum">Кнопка в окне.</param>
    /// <param name="icon">Иконка сообщения.</param>
    public Task<ButtonResult> ShowMessageBoxAsync(string title, 
        string message,
        ButtonEnum buttonEnum = ButtonEnum.Ok,
        Icon icon = Icon.None);
}