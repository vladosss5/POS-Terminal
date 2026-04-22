using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;

namespace Terminal.ViewModels.Pages;

public class AuthOperatorPageViewModel : PageViewModelBase
{
    /// <summary>
    /// Событие при успешной аутентификации.
    /// </summary>
    private Action _actionOnSuccess;
    
    /// <summary>
    /// Событие при ошибке аутентификации.
    /// </summary>
    private Action _actionOnError;
    
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
            
            PasswordPreview = new string('*', value.Length);
            PasswordIsEmpty = string.IsNullOrEmpty(field);
        }
    }

    /// <summary>
    /// Предпросмотр пароля.
    /// </summary>
    public string PasswordPreview
    {
        get; private set => SetProperty(ref field, value);
    }

    /// <summary>
    /// Пароль пустой?
    /// </summary>
    public bool PasswordIsEmpty
    {
        get; private set => SetProperty(ref field, value);
    } = true;
    
    /// <summary>
    /// Коллекция кнопок для авторизации.
    /// </summary>
    public string[] LoginButtons { get; } =
    [
        "7", "8", "9",
        "4", "5", "6",
        "1", "2", "3",
        "00", "0", ",",
    ];
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="actionOnSuccess">Событие при успешной аутентификации.</param>
    /// <param name="actionOnError">Событие при ошибке аутентификации.</param>
    /// <param name="logger"></param>
    public AuthOperatorPageViewModel(
        Action actionOnSuccess,
        Action actionOnError,
        ILogger<PageViewModelBase> logger) 
        : base(logger)
    {
        _actionOnSuccess = actionOnSuccess;
        _actionOnError = actionOnError;

        var confService = App.Services!.GetRequiredService<IConfigurationService>();
        _defaultRemainingSeconds = confService.CurrentSetting.SecondsAuthenticationCanceled;
    }

    /// <summary>
    /// Авторизация администратора.
    /// </summary>
    public async Task Login()
    {
        StopInactivityTimer();

        var authService = App.Services!.GetRequiredService<IAuthService>();

        if (authService.CurrentUser == null)
        {
            _actionOnError.Invoke();
            return;
        }
        
        
            
        
        // var hashPassword = _configurationService.CurrentSetting.ServicePassword;
        // var success = _hashService.VerifyPasswordWithMd5(Password, hashPassword);
        //
        // if (success)
        //     _actionOnSuccess.Invoke();
        // else
        //     _actionOnError.Invoke();
    }

    /// <summary>
    /// Добавить символ к паролю.
    /// </summary>
    /// <param name="element">Символ.</param>
    public void AddCharInPassword(string element)
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