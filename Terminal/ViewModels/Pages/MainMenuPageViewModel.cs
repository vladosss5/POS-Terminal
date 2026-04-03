using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaEdit.Utils;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.DbEntitiesServices;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Enums;
using Terminal.Core.Models;
using Terminal.Data.Context;
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
    
    /// Фабрика экземпляров: <inheritdoc cref="DataContext"/>
    private readonly IDbContextFactory<DataContext> _dbFactory;

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
        IReceiptPrintService receiptPrintService, 
        IDbContextFactory<DataContext> dbFactory) 
        : base(logger)
    {
        _fileExplorer = fileExplorer;
        _logger = logger;
        _authService = authService;
        _shiftService = shiftService;
        _messageBoxService = messageBoxService;
        _receiptPrintService = receiptPrintService;
        _dbFactory = dbFactory;
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

    /// <summary>
    /// Получить отчёт об операциях.
    /// </summary>
    /// <param name="paymentTypes">Тип оплаты.</param>
    /// <param name="issuerList">Номера эмитентов.</param>
    /// <param name="shiftKey">Номер смены.</param>
    /// <param name="elseIssuer">Прочие эмитенты.</param>
    /// <param name="devideOrg"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<List<SalesReportResult>> GetReportAsync(
        IEnumerable<int> paymentTypes, IEnumerable<int> issuerList, 
        int shiftKey, int elseIssuer, bool devideOrg,
        CancellationToken cancellationToken = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);
        
        var reportScript = await App.ReadSqlScriptFromResourceAsync("report.sql");
        
        var ptPlaceholders = paymentTypes.Select((_, i) => $"@pt{i}").ToList();
        var issuerPlaceholders = issuerList.Select((_, i) => $"@iss{i}").ToList();
    
        var finalSql = reportScript
            .Replace("%PaymentType%", string.Join(",", ptPlaceholders))
            .Replace("%IssuerList%", string.Join(",", issuerPlaceholders))
            .Replace("%ShiftKey%", "@ShiftKey")
            .Replace("%DevideOrg%", "@DevideOrg")
            .Replace("%ElseIssuer%", "@ElseIssuer");
        
        var parameters = new List<SqliteParameter>
        {
            new("@ShiftKey", shiftKey),
            new("@ElseIssuer", elseIssuer),
            new("@DevideOrg", devideOrg ? 1 : 0)
        };

        parameters.AddRange(paymentTypes.Select((t, i) => new SqliteParameter($"@pt{i}", t)));
        parameters.AddRange(issuerList.Select((t, i) => new SqliteParameter($"@iss{i}", t)));
        
        var result = await db.Set<SalesReportResult>()
            .FromSqlRaw(finalSql, parameters.ToArray())
            .ToListAsync(cancellationToken: cancellationToken);

        return result;
    }
}