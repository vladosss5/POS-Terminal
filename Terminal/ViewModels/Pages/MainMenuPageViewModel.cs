using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.ViewModels.NavigationService;

namespace Terminal.ViewModels.Pages;

/// <summary>
/// Логика работы страницы главного меню.
/// </summary>
public partial class MainMenuPageViewModel : PageViewModelBase
{
    private readonly ILogger<MainMenuPageViewModel> _logger;
    private readonly IFileExplorer _fileExplorer;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public MainMenuPageViewModel(
        IFileExplorer fileExplorer, 
        ILogger<MainMenuPageViewModel> logger)
    {
        _fileExplorer = fileExplorer;
        _logger = logger;
        Title = "Главная";
    }
    
    /// <summary>
    /// Команда открытия страницы заправки по карте
    /// </summary>
    public void OpenRefuelingByCard()
    {
        Navigation.NavigateTo<RefuelingByCardPageViewModel>();
    }

    
    public async Task CopyDataBaseDirectoryToDownloads()
    {
        _logger.LogInformation("Вызвано копирование");
        await _fileExplorer.CopyDataBaseDirectoryToDownloadsAsync();
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