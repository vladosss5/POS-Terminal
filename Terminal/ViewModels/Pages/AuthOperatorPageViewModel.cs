using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Interfaces;
using Terminal.Dtos;
using Terminal.Services.NavigationService;

namespace Terminal.ViewModels.Pages;

/// <summary>
/// Логика страницы аутентификации оператора.
/// </summary>
public partial class AuthOperatorPageViewModel : PageViewModelBase
{
    /// <inheritdoc cref="IAuthService" />
    private readonly IAuthService _authService;
    
    /// <inheritdoc cref="AuthNavigationParameters" />
    private readonly AuthNavigationParameters _navigationParams;
    
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
    } = "";

    /// <summary>
    /// Предпросмотр пароля.
    /// </summary>
    public string PasswordPreview
    {
        get; private set => SetProperty(ref field, value);
    } = "";

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
        "00", "0", ".",
    ];
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public AuthOperatorPageViewModel(
        ILoggingService logger, 
        IConfigurationService configurationService,
        IAuthService authService, 
        AuthNavigationParameters navigationParams) 
        : base(logger)
    {
        Title = "Пароль оператора";
        
        _authService = authService;
        _navigationParams = navigationParams;
        _defaultRemainingSeconds = configurationService.CurrentSetting.SecondsAuthenticationCanceled;
        
        ResetInactivityTimer();
    }

    /// <summary>
    /// Авторизация администратора.
    /// </summary>
    public void Login()
    {
        StopInactivityTimer();

        var authResult = _authService.AuthenticateOperator(Password);

        if (authResult)
            NavigateOnSuccess();
        else
            NavigateOnFailure();
    }

    /// <summary>
    /// Добавить символы к паролю.
    /// </summary>
    /// <param name="element">Символы.</param>
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
    /// Переход при успешной аутентификации.
    /// </summary>
    private void NavigateOnSuccess()
    {
        var navigateMethod = typeof(INavigationService)
            .GetMethod(nameof(INavigationService.NavigateTo), Type.EmptyTypes)?
            .MakeGenericMethod(_navigationParams.SuccessPageType);

        if (navigateMethod != null)
            navigateMethod.Invoke(Navigation, null);
    }
    
    /// <summary>
    /// Переход при ошибке аутентификации.
    /// </summary>
    private void NavigateOnFailure()
    {
        if (_navigationParams.GoBackOnCancel && Navigation!.CanGoBack)
        {
            Navigation.GoBack();
            return;
        }

        if (_navigationParams.FailurePageType != null)
        {
            var navigateMethod = typeof(INavigationService)
                .GetMethod(nameof(INavigationService.NavigateTo), Type.EmptyTypes)?
                .MakeGenericMethod(_navigationParams.FailurePageType);

            navigateMethod?.Invoke(Navigation, null);
        }
        else
        {
            if (Navigation!.CanGoBack)
                Navigation.GoBack();
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
                Logger.LogError($"Ошибка при работе таймера бездействия:\n{ex.Message}\n{ex.InnerException}");
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