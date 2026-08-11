using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Terminal.Core.Entities.Models;
using Terminal.Core.Enums;
using Terminal.Core.Interfaces;

namespace Terminal.ViewModels.Pages;

/// <summary>
/// Логика авторизации в качестве админа.
/// </summary>
public partial class AdminLoginPageViewModel : PageViewModelBase
{
    /// <inheritdoc cref="IConfigurationService"/>
    private readonly IConfigurationService _configurationService;

    /// <inheritdoc cref="ICryptographyService"/>
    private readonly ICryptographyService _cryptographyService;

    /// <inheritdoc cref="IPopupService"/>
    private readonly IPopupService _popupService;
    
    /// <summary>
    /// Значение таймера по умолчанию.
    /// </summary>
    private readonly int _defaultRemainingSeconds;
    
    /// <summary>
    /// Токен отмены для таймера бездействия.
    /// </summary>
    private CancellationTokenSource? _inactivityCts;
    
    /// <summary>
    /// Пароль в исходном виде.
    /// </summary>
    private string Password
    {
        get;
        set
        {
            if (!SetProperty(ref field, value)) 
                return;
            
            PasswordChar = new string('*', value.Length);
            PasswordIsEmpty = string.IsNullOrEmpty(field);
        }
    } = "";

    /// <summary>
    /// Предпросмотр пароля.
    /// </summary>
    public string PasswordChar
    {
        get;
        private set => SetProperty(ref field, value);
    } = "";

    /// <summary>
    /// Пароль пустой?
    /// </summary>
    public bool PasswordIsEmpty
    {
        get; 
        private set => SetProperty(ref field, value);
    } = true;
    
    /// <summary>
    /// Коллекция кнопок для авторизации.
    /// </summary>
    public string[] LoginButtons { get; private set; } =
    [
        "7", "8", "9",
        "4", "5", "6",
        "1", "2", "3",
        "00", "0", ".",
    ];
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public AdminLoginPageViewModel(
        ILogger<PageViewModelBase> logger,
        IConfigurationService configurationService, 
        ICryptographyService cryptographyService, 
        IPopupService popupService) 
        : base(logger)
    {
        _configurationService = configurationService;
        _cryptographyService = cryptographyService;
        _popupService = popupService;

        _defaultRemainingSeconds = _configurationService.CurrentSetting.SecondsAuthenticationCanceled;

        Title = "Вход админа";
        
        ResetInactivityTimer();
    }

    /// <summary>
    /// Добавить символ к паролю.
    /// </summary>
    /// <param name="element">Символ.</param>
    [RelayCommand]
    private void AddCharInPassword(string element)
    {
        if (!_inactivityCts!.IsCancellationRequested)
            ResetInactivityTimer();

        Password += element;
    }
    
    /// <summary>
    /// Стереть последний символ.
    /// </summary>
    public void RemoveLastChar()
    {
        if (string.IsNullOrWhiteSpace(Password))
            return;
        
        if (!_inactivityCts!.IsCancellationRequested)
            ResetInactivityTimer();
        
        Password = Password[..^1];
    }

    /// <summary>
    /// Очистить пароль.
    /// </summary>
    public void ClearPassword()
    {
        if (!_inactivityCts!.IsCancellationRequested)
            ResetInactivityTimer();
        
        Password = string.Empty;
    }

    /// <summary>
    /// Авторизация администратора.
    /// </summary>
    public void Login()
    {
        StopInactivityTimer();
        
        var hashPassword = _configurationService.SettingsFromPosOffice.ServiceSettings.Password;
        var success = _cryptographyService.VerifyPasswordWithMd5(Password, hashPassword);

        if (success)
        {
            Navigation!.NavigateTo<SettingsPageViewModel>();
        }
        else
        {
            _popupService.ShowCustomPopup(new Popup("Неверный пароль", PopupType.Error));
            Navigation!.NavigateTo<MainMenuPageViewModel>();
        }
    }
    
    /// <summary>
    /// Сброс таймера бездействия.
    /// </summary>
    private void ResetInactivityTimer()
    {
        if (_defaultRemainingSeconds <= 0)
            return;
        
        _inactivityCts?.Cancel();
        _inactivityCts?.Dispose();
        
        _inactivityCts = new CancellationTokenSource();
        
        Task.Run(async () =>
        {
            try
            {
                await Task.Delay(_defaultRemainingSeconds * 1000, _inactivityCts.Token);
                
                if (!_inactivityCts.Token.IsCancellationRequested && IsNavigationInitialized)
                {
                    await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        Logger.LogInformation($"Таймаут бездействия {_defaultRemainingSeconds} секунд, возврат на предыдущую страницу");
                        GoBackCommand.Execute(null);
                    });
                }
            }
            catch (OperationCanceledException)
            {
                Logger.LogDebug("Таймер бездействия сброшен из-за активности пользователя");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Ошибка при работе таймера бездействия");
            }
        }, _inactivityCts.Token);
    }
    
    /// <summary>
    /// Остановить таймер.
    /// </summary>
    private void StopInactivityTimer()
    {
        _inactivityCts?.Cancel();
        _inactivityCts?.Dispose();
        _inactivityCts = null;
    }
}