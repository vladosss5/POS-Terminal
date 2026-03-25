using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Models;

namespace Terminal.ViewModels.Pages;

/// <summary>
/// Логика работы страницы с настройками открытия смены.
/// </summary>
public class SettingsShiftOpeningPageViewModel : PageViewModelBase
{
    /// <inheritdoc cref="IConfigurationService" />
    private readonly IConfigurationService _configurationService;

    /// <summary>
    /// Выбранный вариант времени ожидания ввода пароля.
    /// </summary>
    public TimeoutOption SecondsAuthenticationCanceled 
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
                SaveSecondsAuthenticationCanceled(value.Seconds);
        } 
    }

    /// <summary>
    /// Варианты времени ожидания ввода пароля.
    /// </summary>
    public HashSet<TimeoutOption> TimeoutValues { get; } = 
    [
        new() { Seconds = 10 },
        new() { Seconds = 15 },
        new() { Seconds = 30 },
        new() { Seconds = 60 }
    ];
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public SettingsShiftOpeningPageViewModel(
        ILogger<PageViewModelBase> logger, 
        IConfigurationService configurationService) 
        : base(logger)
    {
        Title = "Открытие смены";
        
        _configurationService = configurationService;
        
        InitializeData();
    }

    /// <summary>
    /// Инициализировать данные.
    /// </summary>
    private void InitializeData()
    {
        var timeOutValue = _configurationService.CurrentSetting.SecondsAuthenticationCanceled;

        SecondsAuthenticationCanceled = new TimeoutOption { Seconds = timeOutValue };
        TimeoutValues.Add(SecondsAuthenticationCanceled);
    }

    /// <summary>
    /// Сохранить выбор времени ожидания ввода пароля.
    /// </summary>
    /// <param name="value">Кол-во секунд.</param>
    private void SaveSecondsAuthenticationCanceled(short value)
    {
        _configurationService.CurrentSetting.SecondsAuthenticationCanceled = value;
    }
}