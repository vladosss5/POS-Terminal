using Microsoft.Extensions.Logging;

namespace Terminal.ViewModels.Pages;

/// <summary>
/// Настройки печати.
/// </summary>
public class SettingsPrintPageViewModel : PageViewModelBase
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public SettingsPrintPageViewModel(
        ILogger<PageViewModelBase> logger) 
        : base(logger)
    {
        Title = "Настройки печати";
    }
    
    
}