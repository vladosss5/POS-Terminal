using Microsoft.Extensions.Logging;
using SQLitePCL;

namespace Terminal.ViewModels.Pages;

public class ShiftReportPageViewModel : PageViewModelBase
{
    /// <summary>
    /// Текст чека.
    /// </summary>
    public string ReceiptText
    {
        get;
        set => SetProperty(ref field, value);
    }
    
    
    public ShiftReportPageViewModel(ILogger<PageViewModelBase> logger) : base(logger)
    {
    }

    public void Close()
    {
        
    }

    public void Print()
    {
        
    }
}