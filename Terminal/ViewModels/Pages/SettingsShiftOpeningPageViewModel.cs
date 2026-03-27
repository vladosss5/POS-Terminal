using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Enums;
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
    public TimeoutOptionDto SecondsAuthenticationCanceled 
    {
        get;
        set
        {
            if (!SetProperty(ref field, value))
                return;
                
            _configurationService.CurrentSetting.SecondsAuthenticationCanceled = value.Seconds;
            _configurationService.SaveSettingsToFile();
        } 
    }

    /// <summary>
    /// Варианты времени ожидания ввода пароля.
    /// </summary>
    public HashSet<TimeoutOptionDto> TimeoutValues { get; } = 
    [
        new() { Seconds = 10 },
        new() { Seconds = 15 },
        new() { Seconds = 30 },
        new() { Seconds = 60 }
    ];
    
    /// <summary>
    /// Варианты аутентификации при открытии смены.
    /// </summary>
    public LoadMode[] LoadModes { get; set; } = Enum.GetValues<LoadMode>();

    /// <summary>
    /// Выбраный вариант аутентификации.
    /// </summary>
    public LoadMode SelectedLoadMode
    {
        get;
        set
        {
            if (!SetProperty(ref field, value))
                return;
            
            _configurationService.CurrentSetting.LoadMode = (int)value;
            _configurationService.SaveSettingsToFile();
        }
    }
    
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

        SecondsAuthenticationCanceled = new TimeoutOptionDto { Seconds = timeOutValue };
        TimeoutValues.Add(SecondsAuthenticationCanceled);

        SelectedLoadMode = (LoadMode)_configurationService.CurrentSetting.LoadMode;
    }
}