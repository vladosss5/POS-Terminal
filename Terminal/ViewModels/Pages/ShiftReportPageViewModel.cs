using System;
using Microsoft.Extensions.Logging;
using SQLitePCL;

namespace Terminal.ViewModels.Pages;

public class ShiftReportPageViewModel : PageViewModelBase
{
    /// <summary>
    /// Событие для печати.
    /// </summary>
    private readonly Action _print;

    /// <summary>
    /// Событие для выхода.
    /// </summary>
    private readonly Action _exit;
    
    
    /// <summary>
    /// Текст чека.
    /// </summary>
    public string ReceiptText
    {
        get;
        set => SetProperty(ref field, value);
    }
    
    
    public ShiftReportPageViewModel(
        ILogger<PageViewModelBase> logger, 
        string receiptText,
        Action? print = null,
        Action? exit = null)
        : base(logger)
    {
        ReceiptText = receiptText;

        if (print != null)
            _print = print;

        if (exit != null)
            _exit = exit;
    }

    public void Close()
    {
        _exit.Invoke();
    }

    public void Print()
    {
        _print.Invoke();
    }
}