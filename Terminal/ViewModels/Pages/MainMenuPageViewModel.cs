using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaEdit.Utils;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MsBox.Avalonia.Enums;
using Terminal.Application.Interfaces.DbEntitiesServices;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Enums;
using Terminal.Core.Models;
using Terminal.Persistence.MainDB;
using Terminal.Dtos;
using Terminal.Persistence.TmsClient;
using Terminal.Services.AuthPageFactory;
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

    ///<inheritdoc cref="IConfigurationService"/>
    private readonly IConfigurationService _configurationService;
    
    /// <inheritdoc cref="IMessageBoxService"/>
    private readonly IMessageBoxService _messageBoxService;

    /// <inheritdoc cref="IReceiptPrintService"/>
    private readonly IReceiptPrintService _receiptPrintService;
    
    /// <inheritdoc cref="IParameterService" />
    private readonly IParameterService _parameterService;
    
    /// <inheritdoc cref="ICryptographyService" />
    private readonly ICryptographyService _cryptographyService;
    
    /// <inheritdoc cref="ITmsClient" />
    private readonly ITmsClient _tmsClient;
    
    /// Фабрика экземпляров: <inheritdoc cref="DataContext"/>
    private readonly IDbContextFactory<DataContext> _dbFactory;
    
    /// Фабрика экземпляров: <inheritdoc cref="IAuthPageFactory"/>
    private readonly IAuthPageFactory _authPageFactory;
    
    
    /// <summary>
    /// Коллекция пунктов главного меню.
    /// </summary>
    public ObservableCollection<MainMenuItemModel> MenuItems { get; } = [];

    
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
        IDbContextFactory<DataContext> dbFactory,
        IConfigurationService configurationService, 
        IAuthPageFactory authPageFactory, 
        IParameterService parameterService,
        ICryptographyService cryptographyService, 
        ITmsClient tmsClient) 
        : base(logger)
    {
        _fileExplorer = fileExplorer;
        _logger = logger;
        _authService = authService;
        _shiftService = shiftService;
        _messageBoxService = messageBoxService;
        _receiptPrintService = receiptPrintService;
        _dbFactory = dbFactory;
        _configurationService = configurationService;
        _authPageFactory = authPageFactory;
        _parameterService = parameterService;
        _cryptographyService = cryptographyService;
        _tmsClient = tmsClient;
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
        var openShift = await _shiftService.GetOpenedShiftOrDefaultAsync();
        var shiftNumber = openShift != null ? openShift.ShiftKey : 0;
        
        var confirmPage = new ConfirmationPageViewModel(
            _logger,
            "Закрыть смену",
            $"№ {shiftNumber}",
            async void () =>
            {
                try
                {
                    _authService.Logout();
        
                    var closingShift = await _shiftService.GetOpenedShiftOrDefaultAsync();
        
                    if (closingShift != null)
                        await _shiftService.CloseShiftAsync(closingShift);
        
                    Navigation.NavigateTo<OpenShiftPageViewModel>();
                }
                catch (Exception e)
                {
                    _logger.LogInformation(e.Message);
                    Navigation.NavigateTo<OpenShiftPageViewModel>();
                }
            },
            () =>
            {
                Navigation.NavigateTo<MainMenuPageViewModel>();
            });
        
        Navigation.NavigateToInstancePage(confirmPage);
    }
    
    /// <summary>
    /// Напечатать промежуточный отчёт за смену.
    /// </summary>
    /// <param name="arg">Токен отмены.</param>
    private async Task PrintInterimReport(CancellationToken arg)
    {
        var openShift = await _shiftService.GetOpenedShiftOrDefaultAsync();

        if (openShift == null)
        {
            await _messageBoxService.ShowMessageBoxAsync("Ошибка", "Ни одна смена не открыта.");
            return;
        }
        
        var issuerNumber = await _parameterService.GetValueAsync(AppParameter.IssuerId);
        
        var divideByIssuers = _configurationService.SettingsFromPosOffice.MainSettings.Print.ReportDivide;
        var paymentTypes = _configurationService.SettingsFromPosOffice.MainSettings.Print.ReportPaymentTypes!
            .Split(',')
            .Select(x => Convert.ToInt32(x));

        var sales = await GetReportAsync(
            paymentTypes: paymentTypes,
            issuerList: [Convert.ToInt32(issuerNumber)], 
            shiftKey: openShift.ShiftKey ?? 0,
            elseIssuer: divideByIssuers ? -1 : Convert.ToInt32(issuerNumber), 
            devideOrg: false, 
            cancellationToken: arg);

        var receiptNumber = await GetNumberLastReceipt(arg);
        
        var terminalNumber = await _parameterService.GetValueAsync(AppParameter.SerialNO111);
        var operatorName = _authService.CurrentUser?.Name;

        var reportData = new ShiftReportDataDto
        {
            ReceiptNumber = receiptNumber,
            IssuerNumber = issuerNumber,
            TerminalNumber = terminalNumber,
            Shift = openShift,
            SalesList = sales,
            OperatorName = !string.IsNullOrEmpty(operatorName) ? operatorName : "undefined"
        };

        await _receiptPrintService.PrintShiftReportAsync(reportData);
    }

    /// <summary>
    /// Возвращает номер последнего чека, инкрементирует и сохраняет в БД.
    /// </summary>
    /// <param name="arg">Токен отмены.</param>
    /// <returns>Номер последнего чека.</returns>
    private async Task<int> GetNumberLastReceipt(CancellationToken arg)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(arg);
        var chekNumberSetting = await db.Settings.FirstOrDefaultAsync(x => x.SettingsKey == SettingsKey.Sale, cancellationToken: arg);

        if (chekNumberSetting == null)
            return 1;
            
        var currentNumber = chekNumberSetting.Value!.Value + 1;
        
        chekNumberSetting.Value = currentNumber;
        db.Update(chekNumberSetting);
            
        await db.SaveChangesAsync(arg);

        return currentNumber;
    }
    
    /// <summary>
    /// Открыть страницу управления ценами ресурсов.
    /// </summary>
    private void OpenResourcePage()
    {
        var authParams = new AuthNavigationParameters
        {
            SuccessPageType = typeof(ResourcePageViewModel),
            FailurePageType = typeof(MainMenuPageViewModel),
            GoBackOnCancel = true
        };

        var authPage = _authPageFactory.Create(authParams);
        Navigation.NavigateToInstancePage(authPage);
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
                Title = "TMS",
                Command = new AsyncRelayCommand(AuthInTmsAsync)
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
                Title = "Инкассация",
            },
            new MainMenuItemModel
            {
                Title = "Смена цены",
                Command = new RelayCommand(OpenResourcePage)
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
    /// Аутентификация клиента в TMS.
    /// </summary>
    private async Task AuthInTmsAsync()
    {
        var terminalNumber = await _parameterService.GetValueAsync(AppParameter.SerialNO111);
        var plainText = terminalNumber + " " + Guid.NewGuid();
        
        var password = _configurationService.CurrentSetting.TmsConfiguration!.Key;
        var salt = _configurationService.CurrentSetting.TmsConfiguration!.Salt;
        
        var workload = _cryptographyService.EncryptAes(plainText, password, Encoding.UTF8.GetBytes(salt));

        await _tmsClient.AuthenticationAsync(workload);

        if (_tmsClient.ConnectionStatus == TmsConnectionStatus.Authorized)
        {
            await _messageBoxService.ShowMessageBoxAsync("Успех", "Авторизация в TMS удачна");
        }
        else
        {
            await _messageBoxService.ShowMessageBoxAsync("Ошибка", "Авторизация в TMS не удачна");
        }
    }

    /// <summary>
    /// Получить отчёт об операциях.
    /// </summary>
    /// <param name="paymentTypes">Тип оплаты.</param>
    /// <param name="issuerList">Номера эмитентов.</param>
    /// <param name="shiftKey">Номер смены.</param>
    /// <param name="elseIssuer">Прочие эмитенты.</param>
    /// <param name="devideOrg">Разделять продажи по эмитентам.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Список элементов сменного отчёта.</returns>
    private async Task<List<SalesReportResult>> GetReportAsync(
        IEnumerable<int> paymentTypes, 
        IEnumerable<int> issuerList, 
        int shiftKey, 
        int elseIssuer, 
        bool devideOrg,
        CancellationToken cancellationToken = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);
        
        var reportScript = await App.ReadSqlScriptFromResourceAsync("report.sql");

        var paymentTypesArray = paymentTypes as int[] ?? paymentTypes.ToArray();
        var ptPlaceholders = paymentTypesArray.Select((_, i) => $"@pt{i}").ToList();
        
        var issuerArray = issuerList as int[] ?? issuerList.ToArray();
        var issuerPlaceholders = issuerArray.Select((_, i) => $"@iss{i}").ToList();
    
        var finalSql = reportScript!
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

        parameters.AddRange(paymentTypesArray.Select((t, i) => new SqliteParameter($"@pt{i}", t)));
        parameters.AddRange(issuerArray.Select((t, i) => new SqliteParameter($"@iss{i}", t)));
        
        var result = await db.Set<SalesReportResult>()
            .FromSqlRaw(finalSql, parameters.ToArray())
            .ToListAsync(cancellationToken: cancellationToken);

        return result;
    }
}