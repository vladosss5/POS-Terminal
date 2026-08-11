using System;
using System.Threading.Tasks;
using Avalonia;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Enums;
using Terminal.Core.Interfaces;

namespace Terminal.ViewModels.Pages;

/// <summary>
/// Логика работы первичной настройки терминала.
/// </summary>
public class InitialSetupPageViewModel : PageViewModelBase
{
    /// <inheritdoc cref="ILogger" />
    private readonly ILogger<InitialSetupPageViewModel> _logger;
    
    /// <inheritdoc cref="IParameterService" />
    private readonly IParameterService _parameterService;

    /// <inheritdoc cref="IParameterService" />
    private readonly IPopupService _popupService;

    /// <summary>
    /// Номер эмитента.
    /// </summary>
    public string IssuerNumber
    {
        get; 
        set => SetProperty(ref field, value);
    }
    
    /// <summary>
    /// Номер терминала.
    /// </summary>
    public string TerminalNumber
    {
        get; 
        set => SetProperty(ref field, value);
    }

    /// <summary>
    /// Адрес TMS.
    /// </summary>
    public string TmsIp
    {
        get;
        set => SetProperty(ref field, value);
    }
    
    /// <summary>
    /// Порт TMS.
    /// </summary>
    public string TmsPort
    {
        get;
        set => SetProperty(ref field, value);
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
    public InitialSetupPageViewModel(
        IParameterService parameterService,
        ILogger<PageViewModelBase> logger, 
        ILogger<InitialSetupPageViewModel> loggerService, 
        IPopupService popupService) 
        : base(logger)
    {
        _parameterService = parameterService;
        _logger = loggerService;
        _popupService = popupService;
        Title = "Первичная настройка";
    }

    /// <summary>
    /// Сохранить настройки.
    /// </summary>
    public async Task SaveSetupAsync()
    {
        try
        {
            await _parameterService.SetValueAsync(AppParameter.IssuerId, IssuerNumber);
            await _parameterService.SetValueAsync(AppParameter.SerialNO111, TerminalNumber);
            await _parameterService.SetValueAsync(AppParameter.TmsIp, TmsIp);
            await _parameterService.SetValueAsync(AppParameter.TmsPort, TmsPort);
            await _parameterService.SetValueAsync(AppParameter.IsInstalled, "1");
        
            Navigation!.NavigateTo<OpenShiftPageViewModel>();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            _popupService.ShowError($"Ошибка сохранения {e.Message}");
        }
    }
}