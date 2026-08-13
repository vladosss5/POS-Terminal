using System;
using System.Collections.ObjectModel;
using Terminal.Core.Interfaces;

namespace Terminal.ViewModels.Pages;

/// <summary>
/// Страница предпоказа сменного чека.
/// </summary>
public class ShiftReportPageViewModel : PageViewModelBase
{
    /// <summary>
    /// Событие для печати.
    /// </summary>
    private readonly Action? _print;

    /// <summary>
    /// Событие для выхода.
    /// </summary>
    private readonly Action? _exit;
    
    /// <summary>
    /// Текст чека построчно.
    /// </summary>
    public ObservableCollection<string> ReceiptLines
    {
        get;
        set => SetProperty(ref field, value);
    }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Сервис логирования.</param>
    /// <param name="receiptText">Текст чека.</param>
    /// <param name="print">Метод вызываемый при отправке чека на печать.</param>
    /// <param name="exit">Метод вызываемый при выходе из предпросмотра.</param>
    public ShiftReportPageViewModel(
        ILoggingService logger, 
        string receiptText,
        Action? print = null,
        Action? exit = null)
        : base(logger)
    {
        ReceiptLines = [.. receiptText.Split('\n')];

        if (print != null)
            _print = print;

        if (exit != null)
            _exit = exit;
    }

    /// <summary>
    /// Закрыть страницу.
    /// </summary>
    public void Close()
    {
        _exit?.Invoke();
    }

    /// <summary>
    /// Напечатать чек.
    /// </summary>
    public void Print()
    {
        _print?.Invoke();
    }
}