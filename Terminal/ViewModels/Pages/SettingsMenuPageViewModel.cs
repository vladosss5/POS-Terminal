using System.Collections.ObjectModel;
using AvaloniaEdit.Utils;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Terminal.ViewModels.Items;

namespace Terminal.ViewModels.Pages;

/// <summary>
/// Логика работы страницы с настройками.
/// </summary>
public class SettingsMenuPageViewModel : PageViewModelBase
{
    /// <summary>
    /// Коллекция пунктов меню.
    /// </summary>
    public ObservableCollection<SettingsMenuItemModel> MenuItems { get; } = [];
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger"></param>
    public SettingsMenuPageViewModel(
        ILogger<PageViewModelBase> logger) 
        : base(logger)
    {
        Title = "Настройки";
        AddItemsIntoMenu();
    }
    
    /// <summary>
    /// Перейти к прошлому шагу.
    /// </summary>
    public void StepBack()
    {
        Navigation.NavigateTo<MainMenuPageViewModel>();
    }

    /// <summary>
    /// Создать кнопки главного меню.
    /// </summary>
    private void AddItemsIntoMenu()
    {
        MenuItems.AddRange([
            new SettingsMenuItemModel
            {
                Title = "Типы оплаты",
                Command = new RelayCommand(delegate
                {
                    Navigation.NavigateTo<PaymentTypesSettingsPageViewModel>();
                })
            },
            new SettingsMenuItemModel
            {
                Title = "Аутентификация",
                Command = new RelayCommand(delegate
                {
                    Navigation.NavigateTo<SettingsShiftOpeningPageViewModel>();
                })
            }
        ]);
    }
}