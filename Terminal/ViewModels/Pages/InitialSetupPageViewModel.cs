using System;
using System.Threading.Tasks;
using Avalonia;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Enums;

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
        ILogger<InitialSetupPageViewModel> loggerService) 
        : base(logger)
    {
        _parameterService = parameterService;
        _logger = loggerService;
        Title = "Первичная настройка";
    }

    /// <summary>
    /// Сохранить настройки.
    /// </summary>
    public async Task SaveSetupAsync()
    {
        try
        {
            await _parameterService.SetValue(AppParameter.IsInstalled, "1");
            await _parameterService.SetValue(AppParameter.IssuerId, IssuerNumber);
            await _parameterService.SetValue(AppParameter.SerialNO111, TerminalNumber);
        
            Navigation!.NavigateTo<OpenShiftPageViewModel>();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }
}