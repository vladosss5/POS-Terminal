using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaEdit.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MsBox.Avalonia;
using Terminal.Application.Interfaces.DbEntitiesServices;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.DbEntities;
using Terminal.Core.Enums;
using Terminal.Core.Models;
using Terminal.Data.Context;
using Terminal.ViewModels.Items;

namespace Terminal.ViewModels.Pages;

public partial class OpenShiftPageViewModel : PageViewModelBase
{
    /// Фабрика экземпляров: <inheritdoc cref="DataContext"/>
    private readonly IDbContextFactory<DataContext> _dbFactory;

    /// <inheritdoc cref="IAuthService"/>
    private readonly IAuthService _authService;
    
    /// <inheritdoc cref="IShiftService"/>
    private readonly IShiftService _shiftService;
    
    /// <inheritdoc cref="ICardReaderService"/>
    private readonly ICardReaderService _cardReaderService;

    /// <summary>
    /// Значение таймера по умолчанию.
    /// </summary>
    private readonly int _defaultRemainingSeconds;
    
    /// <summary>
    /// Делегат для обработки нажатия Enter.
    /// </summary>
    private Action? _onEnterPressedHandler;
    
    /// <summary>
    /// Токен отмены для операций ввода.
    /// </summary>
    private CancellationTokenSource? _inputCancellationTokenSource;
    
    /// <summary>
    /// Токен отмены при истечении времени.
    /// </summary>
    private CancellationTokenSource? _timeoutCancellationTokenSource;

    /// <summary>
    /// Выбранная учётная запись.
    /// </summary>
    private User _selectedUser;
    
    /// <summary>
    /// Индекс текущего шага.
    /// </summary>
    [ObservableProperty] private int _currentStepIndex;

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
    /// Индикатор ожидания ввода.
    /// </summary>
    public bool IsWaitingForInput
    {
        get;
        private set => SetProperty(ref field, value);
    }

    /// <summary>
    /// Оставшееся время в секундах.
    /// </summary>
    private int RemainingSeconds
    {
        get;
        set => SetProperty(ref field, value);
    }
    
    /// <summary>
    /// Коллекция шагов авторизации.
    /// </summary>
    public ObservableCollection<StepViewModelBase> Steps
    {
        get;
        private set => SetProperty(ref field, value);
    }

    /// <summary>
    /// Коллекция пользователей.
    /// </summary>
    public ObservableCollection<User> Users { get; set; } = [];

    /// <summary>
    /// Коллекция кнопок для авторизации.
    /// </summary>
    public LoginButton[] LoginButtons { get; private set; }


    /// <summary>
    /// Конструктор.
    /// </summary>
    public OpenShiftPageViewModel(
        ILogger<PageViewModelBase> logger, 
        IDbContextFactory<DataContext> dbFactory, 
        IAuthService authService, 
        IShiftService shiftService, 
        ICardReaderService cardReaderService, 
        IConfigurationService configurationService) 
        : base(logger)
    {
        _dbFactory = dbFactory;
        _authService = authService;
        _shiftService = shiftService;
        _cardReaderService = cardReaderService;
        
        _defaultRemainingSeconds = Task.Run(() => 
                configurationService.GetValueAsync<int>("SecondsAuthenticationCanceled"))
            .GetAwaiter()
            .GetResult();

        _ = InitializeData();
    }

    /// <summary>
    /// Нажатие на кнопку.
    /// </summary>
    /// <param name="button">Кнопка на которую нажали.</param>
    public async Task ButtonClick(LoginButton button)
    {
        if (!_inputCancellationTokenSource!.IsCancellationRequested)
            ResetInputTimer();
        
        switch (button.Type)
        {
            case LoginButtonTypes.Digit:
                AddCharInPassword(button.Content);
                break;
            case LoginButtonTypes.Enter:
                _onEnterPressedHandler?.Invoke();
                await AuthenticationWithPasswordAsync();
                break;
            case LoginButtonTypes.Backspace:
                RemoveLastChar();
                break;
        }
    }
    
    /// <summary>
    /// Выбрать учётную запись для входа.
    /// </summary>
    /// <param name="user">Учётная запись оператора.</param>
    public void SelectUser(User user)
    {
        _selectedUser = user;
        Steps[0].CompleteStepCommand.ExecuteAsync(null);
        
        StartParallelInput();
    }

    /// <summary>
    /// Переместиться на страницу назад.
    /// </summary>
    public void StepBack()
    {
        Steps[CurrentStepIndex].IsActive = false;
        
        CurrentStepIndex--;
        Title = Steps[CurrentStepIndex].StepName;
        
        var prevStep = Steps[CurrentStepIndex];
        prevStep.IsActive = true;
        prevStep.IsCompleted = false;
    }

    /// <summary>
    /// Добавить символ к паролю.
    /// </summary>
    /// <param name="element">Символ.</param>
    private void AddCharInPassword(string element)
    {
        Password += element;
    }

    /// <summary>
    /// Пройти аутентификацию.
    /// </summary>
    private async Task AuthenticationWithPasswordAsync()
    {
        if (string.IsNullOrEmpty(Password))
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Введите пароль").ShowAsync();
            StartParallelInput();
            return;
        }
        
        CancelAllOperations();
        IsWaitingForInput = false;
        
        var authorizeIsSuccess = await _authService.LoginWithPasswordAsync(_selectedUser.Name!, Password);
        
        if (!authorizeIsSuccess)
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Пароли не совпали").ShowAsync();
            Password = string.Empty;
            StartParallelInput();
            return;
        }
        
        await CompleteAuthorizationAsync();
    }
    
    /// <summary>
    /// Аутентификация по карте.
    /// </summary>
    private async Task AuthenticationWithCard(string cardNumber)
    {
        CancelAllOperations();
        
        IsWaitingForInput = false;

        var intCardNumber = Convert.ToInt32(cardNumber, 16);
        var authorizeIsSuccess = await _authService.LoginWithCardNumber(_selectedUser.Name!, intCardNumber);
        
        if (!authorizeIsSuccess)
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Карта не зарегистрирована").ShowAsync();
            StartParallelInput();
            return;
        }

        await CompleteAuthorizationAsync();
    }
    
    /// <summary>
    /// Завершение авторизации и открытие смены.
    /// </summary>
    private async Task CompleteAuthorizationAsync()
    {
        var openedShift = await _shiftService.GetOpenedShiftOrDefaultAsync();
        
        if (openedShift == null)
            await _shiftService.OpenShiftAsync();
        
        Navigation.NavigateTo<MainMenuPageViewModel>();
    }

    /// <summary>
    /// Запуск параллельного ожидания ввода пароля и карты.
    /// </summary>
    private void StartParallelInput()
    {
        CancelAllOperations();
        
        _inputCancellationTokenSource = new CancellationTokenSource();
        _timeoutCancellationTokenSource = new CancellationTokenSource();
        
        IsWaitingForInput = true;
        RemainingSeconds = _defaultRemainingSeconds;
        
        _onEnterPressedHandler = () =>
        {
            _inputCancellationTokenSource?.Cancel();
        };
        
        _ = StartCountdownTimer(_timeoutCancellationTokenSource.Token);
        _ = WaitForInputParallelAsync(_inputCancellationTokenSource.Token);
    }

    /// <summary>
    /// Ожидание ввода пароля и карты параллельно.
    /// </summary>
    private async Task WaitForInputParallelAsync(CancellationToken cancellationToken)
    {
        try
        {
            var passwordTask = WaitForPasswordInputAsync(cancellationToken);
            var cardTask = WaitForCardInputAsync(cancellationToken);

            var completedTask = await Task.WhenAny(passwordTask, cardTask);

            cancellationToken.ThrowIfCancellationRequested();

            if (completedTask == passwordTask)
            {
                await passwordTask;
            }
            else if (completedTask == cardTask)
            {
                var cardResult = await cardTask;
                if (cardResult.IsSuccess)
                {
                    await AuthenticationWithCard(cardResult.Card!.Uid);
                }
                else if (cardResult.ErrorType == CardReaderErrorType.Timeout)
                {
                    await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Вышло время ожидания").ShowAsync();
                    Navigation.NavigateTo<OpenShiftPageViewModel>();
                }
                else if (cardResult.ErrorMessage != null)
                {
                    await MessageBoxManager.GetMessageBoxStandard("Ошибка", cardResult.ErrorMessage).ShowAsync();
                    StartParallelInput();
                }
            }
        }
        catch(OperationCanceledException)
        {
            // Ignore
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Ошибка при ожидании ввода");
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", $"Произошла ошибка: {e.Message}").ShowAsync();
        }
        finally
        {
            IsWaitingForInput = false;
        }
    }

    /// <summary>
    /// Ожидание ввода пароля с клавиатуры.
    /// </summary>
    private Task<string> WaitForPasswordInputAsync(CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<string>();
        
        cancellationToken.Register(() =>
        {
            tcs.TrySetCanceled(cancellationToken);
        });
    
        return tcs.Task;
    }

    /// <summary>
    /// Ожидание считывания карты.
    /// </summary>
    private async Task<CardReadResult> WaitForCardInputAsync(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _cardReaderService.ReadCardAsync(RemainingSeconds, cancellationToken);
            return result;
        }
        catch (OperationCanceledException)
        {
            return CardReadResult.Cancelled();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Ошибка при считывании карты");
            return CardReadResult.ServiceError(ex.Message);
        }
    }

    /// <summary>
    /// Запуск таймера обратного отсчёта.
    /// </summary>
    private async Task StartCountdownTimer(CancellationToken cancellationToken)
    {
        try
        {
            while (RemainingSeconds > 0 && !cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000, cancellationToken);
                RemainingSeconds--;
            }
            
            if (RemainingSeconds <= 0 && !cancellationToken.IsCancellationRequested)
            {
                _inputCancellationTokenSource?.Cancel();
            }
        }
        catch (OperationCanceledException)
        {
            Logger.LogInformation("Таймер отменён.");
        }
    }
    
    /// <summary>
    /// Сброс таймера ввода.
    /// </summary>
    private void ResetInputTimer()
    {
        _timeoutCancellationTokenSource?.Cancel();
        _timeoutCancellationTokenSource = new CancellationTokenSource();
        RemainingSeconds = _defaultRemainingSeconds;
        
        _ = StartCountdownTimer(_timeoutCancellationTokenSource.Token);
    }
    
    /// <summary>
    /// Отмена всех операций.
    /// </summary>
    private void CancelAllOperations()
    {
        _inputCancellationTokenSource?.Cancel();
        _inputCancellationTokenSource?.Dispose();
        _inputCancellationTokenSource = null;
        
        _timeoutCancellationTokenSource?.Cancel();
        _timeoutCancellationTokenSource?.Dispose();
        _timeoutCancellationTokenSource = null;
        
        _onEnterPressedHandler = null;
    }

    /// <summary>
    /// Удалить последний символ из пароля.
    /// </summary>
    private void RemoveLastChar()
    {
        Password = Password[..^1];
    }
    
    /// <summary>
    /// Инициаизировать данные.
    /// </summary>
    private async Task InitializeData()
    {
        Steps = [
            new StepViewModelBase("Кассиры", OnStepCompleted),
            new StepViewModelBase("Пароль", OnStepCompleted)
        ];

        Title = Steps[0].StepName;
        Steps[0].IsActive = true;

        await using var db = await _dbFactory.CreateDbContextAsync();

        var users = await db.Users.ToListAsync();
        Users.AddRange(users);
        
        LoginButtons =
        [
            new() { Content = "7", ContentIsImage = false, Type = LoginButtonTypes.Digit},
            new() { Content = "8", ContentIsImage = false, Type = LoginButtonTypes.Digit },
            new() { Content = "9", ContentIsImage = false, Type = LoginButtonTypes.Digit },
            new() { Content = "4", ContentIsImage = false, Type = LoginButtonTypes.Digit },
            new() { Content = "5", ContentIsImage = false, Type = LoginButtonTypes.Digit },
            new() { Content = "6", ContentIsImage = false, Type = LoginButtonTypes.Digit },
            new() { Content = "1", ContentIsImage = false, Type = LoginButtonTypes.Digit },
            new() { Content = "2", ContentIsImage = false, Type = LoginButtonTypes.Digit },
            new() { Content = "3", ContentIsImage = false, Type = LoginButtonTypes.Digit },
            new() { Content = "backspace.png", ContentIsImage = true, Type = LoginButtonTypes.Backspace },
            new() { Content = "0", ContentIsImage = false, Type = LoginButtonTypes.Digit },
            new() { Content = "enter.png", ContentIsImage = true, Type = LoginButtonTypes.Enter }
        ];
    }
    
    /// <summary>
    /// Пометить шаг выполненным.
    /// </summary>
    private async Task OnStepCompleted()
    {
        CurrentStepIndex++;
        Title = Steps[CurrentStepIndex].StepName;
        Steps[CurrentStepIndex].IsActive = true;
    }
}