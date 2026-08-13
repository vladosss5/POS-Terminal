using System;
using Terminal.Core.Interfaces;

namespace Terminal.ViewModels.Pages;

/// <summary>
/// Страница для подтверждения действия.
/// </summary>
public class ConfirmationPageViewModel : PageViewModelBase
{
    /// <summary>
    /// Событие для печати.
    /// </summary>
    private readonly Action _confirm;

    /// <summary>
    /// Событие для выхода.
    /// </summary>
    private readonly Action _exit;
    
    /// <summary>
    /// Текст подтверждения.
    /// </summary>
    public string ConfirmationText 
    { 
        get;
        set => SetProperty(ref field, value); 
    }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Сервис логирования.</param>
    /// <param name="title">Заголовок.</param>
    /// <param name="confirmationText">Текст подтверждения.</param>
    /// <param name="confirm">Событие для подтверждения.</param>
    /// <param name="exit">Событие для отмены.</param>
    public ConfirmationPageViewModel(
        ILoggingService logger,
        string title,
        string confirmationText,
        Action confirm,
        Action exit) 
        : base(logger)
    {
        Title = title;
        ConfirmationText = confirmationText;
        _confirm = confirm;
        _exit = exit;
    }

    /// <summary>
    /// Метод вызывающий подтверждение действия.
    /// </summary>
    public void Confirm()
    {
        _confirm.Invoke();
    }

    /// <summary>
    /// Метод вызывающий отмену действия.
    /// </summary>
    public void Exit()
    {
        _exit.Invoke();
    }
}