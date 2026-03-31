using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MsBox.Avalonia;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Enums;
using Terminal.Core.Models;

namespace Terminal.ViewModels.Pages;

/// <summary>
/// Логика авторизации в качестве админа.
/// </summary>
public class AdminLoginPageViewModel : PageViewModelBase
{
    /// <inheritdoc cref="IConfigurationService"/>
    private readonly IConfigurationService _configurationService;

    /// <inheritdoc cref="IConfigurationService"/>
    private readonly IHashService _hashService;
    
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
    }

    /// <summary>
    /// Предпросмотр пароля.
    /// </summary>
    public string PasswordChar
    {
        get;
        private set => SetProperty(ref field, value);
    }

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
    public LoginButton[] LoginButtons { get; private set; }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public AdminLoginPageViewModel(
        ILogger<PageViewModelBase> logger,
        IConfigurationService configurationService, 
        IHashService hashService) 
        : base(logger)
    {
        _configurationService = configurationService;
        _hashService = hashService;

        _defaultRemainingSeconds = _configurationService.CurrentSetting.SecondsAuthenticationCanceled;

        InitializeData();
    }

    /// <summary>
    /// Обработка нажатия на кнопку.
    /// </summary>
    /// <param name="button">Кнопка.</param>
    public async Task ButtonClick(LoginButton button)
    {
        switch (button.Type)
        {
            case LoginButtonTypes.Enter:
                await Login();
                break;
            case LoginButtonTypes.Digit:
                Password += button.Content;;
                break;
            case LoginButtonTypes.Backspace:
                Password = Password[..^1];
                break;
            default:
                Logger.LogWarning("Нажатая кнопка не определена.");
                break;
        }
    }

    /// <summary>
    /// Авторизация администратора.
    /// </summary>
    private async Task Login()
    {
        StopInactivityTimer();
        
        var hashPassword = _configurationService.CurrentSetting.ServicePassword;
        var success = _hashService.VerifyPasswordWithMd5(Password, hashPassword);

        if (success)
        {
            Navigation.NavigateTo<SettingsMenuPageViewModel>();
        }
        else
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Неверный пароль").ShowAsync();
            Navigation.NavigateTo<MainMenuPageViewModel>();
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
    
    /// <summary>
    /// Инициализировать данные.
    /// </summary>
    private void InitializeData()
    {
        Title = "Вход админа";
        
        LoginButtons =
        [
            new LoginButton { Content = "7", ContentIsImage = false, Type = LoginButtonTypes.Digit},
            new LoginButton { Content = "8", ContentIsImage = false, Type = LoginButtonTypes.Digit },
            new LoginButton { Content = "9", ContentIsImage = false, Type = LoginButtonTypes.Digit },
            new LoginButton { Content = "4", ContentIsImage = false, Type = LoginButtonTypes.Digit },
            new LoginButton { Content = "5", ContentIsImage = false, Type = LoginButtonTypes.Digit },
            new LoginButton { Content = "6", ContentIsImage = false, Type = LoginButtonTypes.Digit },
            new LoginButton { Content = "1", ContentIsImage = false, Type = LoginButtonTypes.Digit },
            new LoginButton { Content = "2", ContentIsImage = false, Type = LoginButtonTypes.Digit },
            new LoginButton { Content = "3", ContentIsImage = false, Type = LoginButtonTypes.Digit },
            new LoginButton { Content = "backspace.png", ContentIsImage = true, Type = LoginButtonTypes.Backspace },
            new LoginButton { Content = "0", ContentIsImage = false, Type = LoginButtonTypes.Digit },
            new LoginButton { Content = "enter.png", ContentIsImage = true, Type = LoginButtonTypes.Enter }
        ];
        
        ResetInactivityTimer();
    }
}