using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AvaloniaEdit.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MsBox.Avalonia;
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

    /// <inheritdoc cref="IHashService"/>
    private readonly IHashService _hashService;

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
            if (SetProperty(ref field, value))
                PasswordChar = new string('*', value.Length);
        }
    }

    /// <summary>
    /// Предпросмотр пароля.
    /// </summary>
    public string PasswordChar
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

    public LoginButton[] LoginButtons { get; private set; }


    /// <summary>
    /// Конструктор.
    /// </summary>
    public OpenShiftPageViewModel(
        ILogger<PageViewModelBase> logger, 
        IDbContextFactory<DataContext> dbFactory, IHashService hashService) 
        : base(logger)
    {
        _dbFactory = dbFactory;
        _hashService = hashService;

        _ = InitializeData();
    }

    public async Task ButtonClick(LoginButton button)
    {
        switch (button.Type)
        {
            case LoginButtonTypes.Digit:
                AddCharInPassword(button.Content);
                break;
            case LoginButtonTypes.Enter:
                await Authentication();
                break;
            case LoginButtonTypes.Backspace:
                RemoveLastChar();
                break;
        }
    }
    
    public void SelectUser(User user)
    {
        _selectedUser = user;
        Steps[0].CompleteStepCommand.ExecuteAsync(null);
    }

    public void StepBack()
    {
        Steps[CurrentStepIndex].IsActive = false;
        
        CurrentStepIndex--;
        Title = Steps[CurrentStepIndex].StepName;
        
        var prevStep = Steps[CurrentStepIndex];
        prevStep.IsActive = true;
        prevStep.IsCompleted = false;
    }

    private void AddCharInPassword(string element)
    {
        Password += element;
    }

    private async Task Authentication()
    {
        if (!_hashService.VerifyPasswordWithMd5(Password, _selectedUser.UserPassword!))
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Пароли не совпали").ShowAsync();
            return;
        }
        
        Navigation.NavigateTo<MainMenuPageViewModel>();
    }

    private void RemoveLastChar()
    {
        Password = Password[..^1];
    }
    
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