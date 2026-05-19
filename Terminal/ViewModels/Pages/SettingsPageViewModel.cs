using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Enums;
using Terminal.Core.Models;
using Terminal.Core.Models.Settings;
using Terminal.Core.Models.SettingsFromPosOffice;
using Terminal.ViewModels.Items;

namespace Terminal.ViewModels.Pages;

/// <summary>
/// Логика работы страницы с настройками.
/// </summary>
public class SettingsPageViewModel : PageViewModelBase
{
    /// <inheritdoc cref="IConfigurationService" />
    private readonly IConfigurationService _configurationService;

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
    /// Коллекция пунктов меню.
    /// </summary>
    public ObservableCollection<SettingsMenuItemModel> MenuItems { get; } = [];
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger"></param>
    public SettingsPageViewModel(
        ILogger<PageViewModelBase> logger,
        IConfigurationService configurationService) 
        : base(logger)
    {
        _configurationService = configurationService;
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
    private void InitializeData()
    {
        var timeOutValue = _configurationService.CurrentSetting.SecondsAuthenticationCanceled;

        SecondsAuthenticationCanceled = new TimeoutOptionDto { Seconds = timeOutValue };
        TimeoutValues.Add(SecondsAuthenticationCanceled);

        SelectedAuthorizeType = (AuthorizeType)_configurationService.SettingsFromPosOffice.MainSettings.Mode.AuthorizeMethod;
    }
}