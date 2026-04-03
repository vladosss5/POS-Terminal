using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaEdit.Utils;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.DbEntitiesServices;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Enums;
using Terminal.Services.NavigationService;
using Terminal.ViewModels.Items;

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

    ///<inheritdoc cref="IAuthService"/>
    private readonly IAuthService _authService;

    ///<inheritdoc cref="IShiftService"/>
    private readonly IShiftService _shiftService;
    
    /// <inheritdoc cref="IMessageBoxService"/>
    private readonly IMessageBoxService _messageBoxService;

    /// <inheritdoc cref="IReceiptPrintService"/>
    private readonly IReceiptPrintService _receiptPrintService;

    /// <summary>
    /// Коллекция пунктов главного меню.
    /// </summary>
    public ObservableCollection<MainMenuItemModel> MenuItems { get; } = new();

    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public MainMenuPageViewModel(
        IFileExplorer fileExplorer, 
        ILogger<MainMenuPageViewModel> logger, 
        IAuthService authService, 
        IShiftService shiftService, 
        IMessageBoxService messageBoxService, 
        IReceiptPrintService receiptPrintService) 
        : base(logger)
    {
        _fileExplorer = fileExplorer;
        _logger = logger;
        _authService = authService;
        _shiftService = shiftService;
        _messageBoxService = messageBoxService;
        _receiptPrintService = receiptPrintService;
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
    /// Скопировать директорию с БД в директорию загрузок.
    /// </summary>
    private async Task CopyDataBaseDirectoryToDownloads()
    {
        _logger.LogInformation("Вызвано копирование");
        await _fileExplorer.CopyDataBaseDirectoryToDownloadsAsync();

        await _messageBoxService.ShowMessageBoxAsync("Успех", "Каталог скопирован!");
    }
    
    /// <summary>
    /// Закрыть смену.
    /// </summary>
    private async Task ShiftClose()
    {
        await _authService.LogoutAsync();

        var openShift = await _shiftService.GetOpenedShiftOrDefaultAsync();

        if (openShift != null)
            await _shiftService.CloseShiftAsync(openShift);

        Navigation.NavigateTo<OpenShiftPageViewModel>();
    }
    
    /// <summary>
    /// Напечатать промежуточный отчёт за смену.
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    private async Task PrintInterimReport(CancellationToken arg)
    {
        var openShift = await _shiftService.GetOpenedShiftOrDefaultAsync();

        if (openShift == null)
            await _messageBoxService.ShowMessageBoxAsync("Ошибка", "Ни одна смена не открыта.");

        await _receiptPrintService.PrintShiftReportAsync(openShift!, ShiftReportType.Interim);
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
                Command = new RelayCommand(delegate
                {
                    Navigation.NavigateTo<SaleProcessPageViewModel>();
                })
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
                Title = "Закрыть смену",
                Command = new AsyncRelayCommand(ShiftClose)
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
              Title  = "Печать чека",
              Command = new RelayCommand(delegate
              {
                  Navigation.NavigateTo<PrintingReceiptPageViewModel>();
              })
            },
            new MainMenuItemModel
            {
                Title = "Пром. отчёт",
                Command = new AsyncRelayCommand(PrintInterimReport)
            },
            new MainMenuItemModel
            {
                Title = "Настройки",
                Command = new RelayCommand(delegate
                {
                    Navigation.NavigateTo<AdminLoginPageViewModel>();
                })
            },
            new MainMenuItemModel
            {
                Title = "Копировать БД", 
                Command = new AsyncRelayCommand(CopyDataBaseDirectoryToDownloads)
            }
        ]);
    }
}