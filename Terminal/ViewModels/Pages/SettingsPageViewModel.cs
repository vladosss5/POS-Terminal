using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Avalonia;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Enums;
using Terminal.Core.Models;
using Terminal.Core.Models.Settings;
using Terminal.Core.Models.SettingsFromPosOffice;

namespace Terminal.ViewModels.Pages;

/// <summary>
/// Логика работы страницы с настройками.
/// </summary>
public partial class SettingsPageViewModel : PageViewModelBase
{
    /// <inheritdoc cref="IConfigurationService" />
    private readonly IConfigurationService _configurationService;
    
    /// <inheritdoc cref="IParameterService" />
    private readonly IParameterService _parameterService;

    /// <summary>
    /// Ссылка на экземпляр класса настроек приложения.
    /// </summary>
    public SettingsModel SettingsModel { get; }
    
    /// <summary>
    /// Ссылка на экземпляр класса настроек приложения из Pos Office.
    /// </summary>
    public SettingsFromPosOffice SettingsFromPosOffice { get; }
    
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
    public AuthorizeType[] LoadModes { get; set; } = Enum.GetValues<AuthorizeType>();

    /// <summary>
    /// Выбранный вариант аутентификации.
    /// </summary>
    public AuthorizeType SelectedAuthorizeType
    {
        get;
        set
        {
            if (!SetProperty(ref field, value))
                return;
            
            _configurationService.SettingsFromPosOffice.MainSettings.Mode.AuthorizeMethod = (int)value;
            _configurationService.SaveSettingsToFile();
        }
    }
    
    /// <summary>
    /// Ip адрес TMS. Наблюдаемое св-во. При изменении автоматически сохраняет в значение БД.
    /// </summary>
    public string TmsIpAddress 
    { 
        get; 
        set
        {
            if (!SetProperty(ref field, value)) 
                return;
            
            if (IpRegex().IsMatch(field))
                _parameterService.SetValueAsync(AppParameter.TmsIp, field);
        }
    }
    
    /// <summary>
    /// Порт TMS. Наблюдаемое св-во. При изменении автоматически сохраняет в значение БД.
    /// </summary>
    public string TmsPort
    {
        get;
        set
        {
            if (!SetProperty(ref field, value))
                return;

            if (PortRegex().IsMatch(field))
                _parameterService.SetValueAsync(AppParameter.TmsPort, field);
        }
    }
    
    /// <summary>
    /// Безопасная зона для интерфейсов при открытии клавиатуры.
    /// </summary>
    public Thickness SafeArea
    {
        get;
        set => SetProperty(ref field, value);
    }

    /// <summary>
    /// Конструктор.
    /// </summary>
    public SettingsPageViewModel(
        ILogger<PageViewModelBase> logger,
        IConfigurationService configurationService, 
        IParameterService parameterService) 
        : base(logger)
    {
        _configurationService = configurationService;
        _parameterService = parameterService;
        Title = "Настройки";
        
        SettingsModel = _configurationService.CurrentSetting;
        SettingsFromPosOffice = _configurationService.SettingsFromPosOffice;
        
        InitializeData();
    }
    
    /// <summary>
    /// Перейти к прошлому шагу.
    /// </summary>
    public void StepBack() => Navigation.NavigateTo<MainMenuPageViewModel>();
    
    /// <summary>
    /// Вызвать сохранение конфигурации в файл.
    /// </summary>
    public void SaveCommand() => _configurationService.SaveSettingsToFile();
    
    /// <summary>
    /// Инициализировать данные.
    /// </summary>
    private async void InitializeData()
    {
        var timeOutValue = _configurationService.CurrentSetting.SecondsAuthenticationCanceled;

        SecondsAuthenticationCanceled = new TimeoutOptionDto { Seconds = timeOutValue };
        TimeoutValues.Add(SecondsAuthenticationCanceled);

        SelectedAuthorizeType = (AuthorizeType)_configurationService.SettingsFromPosOffice.MainSettings.Mode.AuthorizeMethod;

        TmsIpAddress = await _parameterService.GetValueAsync(AppParameter.TmsIp);
        TmsPort = await _parameterService.GetValueAsync(AppParameter.TmsPort);
    }

    [GeneratedRegex(@"^((25[0-5]|2[0-4]\d|1?\d?\d)\.){3}(25[0-5]|2[0-4]\d|1?\d?\d)$")]
    private static partial Regex IpRegex();
    
    [GeneratedRegex(@"^(6553[0-5]|655[0-2]\d|65[0-4]\d{2}|6[0-4]\d{3}|[1-5]\d{4}|[1-9]\d{1,3}|\d)$")]
    private static partial Regex PortRegex();
}