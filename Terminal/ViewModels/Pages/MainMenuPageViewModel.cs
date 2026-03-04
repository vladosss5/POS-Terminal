using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using AvaloniaEdit.Utils;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MsBox.Avalonia;
using Terminal.Application.Interfaces.Services;
using Terminal.ViewModels.Items;
using Terminal.ViewModels.NavigationService;

namespace Terminal.ViewModels.Pages;

/// <summary>
/// Логика работы страницы главного меню.
/// </summary>
public partial class MainMenuPageViewModel : PageViewModelBase
{
    ///<inheritdoc cref="ILogger"/>
    private readonly ILogger<MainMenuPageViewModel> _logger;
    
    ///<inheritdoc cref="IFileExplorer"/>
    private readonly IFileExplorer _fileExplorer;

    /// <summary>
    /// Коллекция пунктов главного меню.
    /// </summary>
    public ObservableCollection<MainMenuItemModel> MenuItems { get; } = new();
    
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public MainMenuPageViewModel(
        IFileExplorer fileExplorer, 
        ILogger<MainMenuPageViewModel> logger) 
        : base(logger)
    {
        _fileExplorer = fileExplorer;
        _logger = logger;
        Title = "Главная";

        AddItemsIntoMenu();
    }
    
    /// <summary>
    /// Этот метод вызывается при активации страницы.
    /// </summary>
    public override void OnActivated(INavigationService navigationService)
    {
        base.OnActivated(navigationService);
        _logger.LogInformation("MainMenuPageViewModel activated");
    }
    
    /// <summary>
    /// Команда открытия страницы заправки по карте.
    /// </summary>
    private void OpenRefuelingByCard()
    {
        Navigation.NavigateTo<RefuelingByCardPageViewModel>();
    }

    /// <summary>
    /// Скопировать директорию с БД в директорию загрузок.
    /// </summary>
    private async Task CopyDataBaseDirectoryToDownloads()
    {
        _logger.LogInformation("Вызвано копирование");
        await _fileExplorer.CopyDataBaseDirectoryToDownloadsAsync();

        await MessageBoxManager.GetMessageBoxStandard("Успех", "Каталог скопирован!").ShowAsync();
    }

    /// <summary>
    /// Создать кнопки главного меню.
    /// </summary>
    private void AddItemsIntoMenu()
    {
        MenuItems.AddRange([
            new MainMenuItemModel
            {
                Title = "Заправка", 
                Command = new RelayCommand(OpenRefuelingByCard)
            },
            new MainMenuItemModel
            {
                Title = "Возврат на карту"
            },
            new MainMenuItemModel
            {
                Title = "Возврат на счёт"
            },
            new MainMenuItemModel
            {
                Title = "Инфо по карте"
            },
            new MainMenuItemModel
            {
                Title = "Закрыть смену"
            },
            new MainMenuItemModel
            {
                Title = "Инкассация"
            },
            new MainMenuItemModel
            {
                Title = "Меню оператора"
            },
            new MainMenuItemModel
            {
                Title = "Пром. отчёт"
            },
            new MainMenuItemModel
            {
                Title = "Настройка"
            },
            new MainMenuItemModel
            {
                Title = "Копировать БД", 
                Command = new AsyncRelayCommand(CopyDataBaseDirectoryToDownloads)
            }
        ]);
    }
}