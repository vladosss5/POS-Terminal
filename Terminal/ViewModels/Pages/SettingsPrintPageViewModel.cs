using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Models.Settings;

namespace Terminal.ViewModels.Pages;

/// <summary>
/// Настройки печати.
/// </summary>
public class SettingsPrintPageViewModel : PageViewModelBase
{
    /// <inheritdoc cref="IConfigurationService" />
    private readonly IConfigurationService _configurationService;

    /// <summary>
    /// Св-во для получения ссылки на экземпляр класса текущей конфигурации.
    /// </summary>
    public SettingsModel SettingsModel { get; }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public SettingsPrintPageViewModel(
        ILogger<PageViewModelBase> logger, 
        IConfigurationService configurationService) 
        : base(logger)
    {
        _configurationService = configurationService;
        Title = "Настройки печати";

        SettingsModel = _configurationService.CurrentSetting;
    }
    
    /// <summary>
    /// Вызвать сохранение конфигурации в файл.
    /// </summary>
    public void SaveCommand() => _configurationService.SaveSettingsToFile();
}