using System;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using Terminal.ViewModels.NavigationService;

namespace Terminal.ViewModels.Pages;

/// <summary>
/// Логика работы страницы главного меню.
/// </summary>
public partial class MainMenuPageViewModel : PageViewModelBase
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public MainMenuPageViewModel()
    {
        Title = "Главная";
    }
    
    /// <summary>
    /// Команда открытия страницы заправки по карте
    /// </summary>
    [RelayCommand]
    private void OpenRefuelingByCard()
    {
        Navigation.NavigateTo<RefuelingByCardPageViewModel>();
    }
    
    /// <summary>
    /// Этот метод вызывается при активации страницы
    /// </summary>
    public override void OnActivated(INavigationService navigationService)
    {
        base.OnActivated(navigationService);
        Debug.WriteLine("MainMenuPageViewModel activated");
    }
}