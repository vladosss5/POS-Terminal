using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaEdit.Utils;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Entities.Models;
using Terminal.Core.Enums;
using Terminal.Core.Interfaces;
using Terminal.Dtos;
using Terminal.Persistence.MainDB;
using Terminal.Services.AuthPageFactory;
using Terminal.Services.NavigationService;
using Terminal.ViewModels.Items;

namespace Terminal.ViewModels.Pages;

/// <summary>
/// Логика работы страницы главного меню.
/// </summary>
public class MainMenuPageViewModel : PageViewModelBase
{
    ///<inheritdoc cref="IFileExplorer"/>
    private readonly IFileExplorer _fileExplorer;

    ///<inheritdoc cref="IAuthService"/>
    private readonly IAuthService _authService;

    ///<inheritdoc cref="IShiftService"/>
    private readonly IShiftService _shiftService;

    ///<inheritdoc cref="IConfigurationService"/>
    private readonly IConfigurationService _configurationService;

    /// <inheritdoc cref="IReceiptPrintService"/>
    private readonly IReceiptPrintService _receiptPrintService;
    
    /// <inheritdoc cref="IParameterService" />
    private readonly IParameterService _parameterService;
    
    /// <inheritdoc cref="IEncashmentService" />
    private readonly IEncashmentService _encashmentService;

    /// <inheritdoc cref="IUpdateInstallerService" />
    private readonly IUpdateInstallerService _installerService;
    
    /// <inheritdoc cref="IStatusNotifierService" />
    private readonly IStatusNotifierService _statusNotifierService;

    /// <inheritdoc cref="IConfigurationUpdatingService" />
    private readonly IConfigurationUpdatingService _configurationUpdatingService;
    
    /// Фабрика экземпляров: <inheritdoc cref="DataContext"/>
    private readonly IDbContextFactory<DataContext> _dbFactory;
    
    /// Фабрика экземпляров: <inheritdoc cref="IAuthPageFactory"/>
    private readonly IAuthPageFactory _authPageFactory;

    /// <inheritdoc cref="IPopupService" />
    private readonly IPopupService _popupService;
    
    
    /// <summary>
    /// Название файла-иконки загрузки.
    /// </summary>
    private const string DownloadIconName = "downloading-file.png";
    
    /// <summary>
    /// Название файла-иконки ошибки загрузки.
    /// </summary>
    private const string AbortedIconName = "aborted.png";
    
    /// <summary>
    /// Название файла-иконки выполненной загрузки.
    /// </summary>
    private const string CompletedIconName = "done.png";
    
    
    /// <summary>
    /// Коллекция пунктов главного меню.
    /// </summary>
    public ObservableCollection<MainMenuItemModel> MenuItems { get; } = [];

    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public MainMenuPageViewModel(
        IFileExplorer fileExplorer, 
        ILoggingService logger, 
        IAuthService authService, 
        IShiftService shiftService, 
        IReceiptPrintService receiptPrintService, 
        IDbContextFactory<DataContext> dbFactory,
        IConfigurationService configurationService, 
        IAuthPageFactory authPageFactory, 
        IParameterService parameterService, 
        IEncashmentService encashmentService,
        IUpdateInstallerService installerService, 
        IConfigurationUpdatingService configurationUpdatingService, 
        IStatusNotifierService statusNotifierService, 
        IPopupService popupService) 
        : base(logger)
    {
        _fileExplorer = fileExplorer;
        _authService = authService;
        _shiftService = shiftService;
        _receiptPrintService = receiptPrintService;
        _dbFactory = dbFactory;
        _configurationService = configurationService;
        _authPageFactory = authPageFactory;
        _parameterService = parameterService;
        _encashmentService = encashmentService;
        _installerService = installerService;
        _configurationUpdatingService = configurationUpdatingService;
        _statusNotifierService = statusNotifierService;
        _popupService = popupService;
        Title = "Главная";
        
        AddItemsIntoMenu();
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
                Command = new RelayCommand(delegate { Navigation!.NavigateTo<SellingProcessPageViewModel>(); })
            },
            new MainMenuItemModel
            {
                Title = "Закрыть смену",
                Command = new AsyncRelayCommand(ShiftClose)
            },
            new MainMenuItemModel
            {
                Title = "Инкассация",
                Command = new AsyncRelayCommand(EncashmentAsync)
            },
            new MainMenuItemModel
            {
                Title = "Смена цены",
                Command = new RelayCommand(OpenResourcePage)
            },
            new MainMenuItemModel
            {
              Title  = "Печать чека",
              Command = new RelayCommand(delegate { Navigation!.NavigateTo<PrintingReceiptPageViewModel>(); })
            },
            new MainMenuItemModel
            {
                Title = "Пром. отчёт",
                Command = new AsyncRelayCommand(PrintInterimReport)
            },
            new MainMenuItemModel
            {
                Title = "Настройки", 
                Command = new RelayCommand(delegate { Navigation!.NavigateTo<AdminLoginPageViewModel>(); })
            },
            new MainMenuItemModel
            {
                Title = "Проверить обновления", 
                Command = new AsyncRelayCommand(UpdateApplicationAsync)
            },
            new MainMenuItemModel
            {
                Title = "Копировать БД", 
                Command = new AsyncRelayCommand(CopyDataBaseDirectoryToDownloads)
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
            }
        ]);
    }

    /// <summary>
    /// Обновить приложение.
    /// </summary>
    private async Task UpdateApplicationAsync()
    {
        try
        {
            Logger.LogInformation("Checking for updates...");

            if (!await _installerService.CheckForUpdates())
                throw new Exception("Обновлений не найдено");

            Logger.LogInformation("New version found. Downloading...");
            UpdateDownloadingStatus(DownloadStatus.InProcess);
            
            await _installerService.DownloadUpdatingFileAsync();
            
            Logger.LogInformation("Update downloaded successfully");
            UpdateDownloadingStatus(DownloadStatus.Completed);
            
            // var confirmedResult = await _messageBoxService // TODO: Сюда диалоговое окно.
            //     .ShowMessageBoxAsync("Инфо", "Загружено обновление. Хотите установить сейчас?", ButtonEnum.YesNo, Icon.Info);
            //
            // if(confirmedResult == ButtonResult.Yes)
            
            await _installerService.InstallUpdatingPatchAsync();
        }
        catch (Exception e)
        {
            _popupService.ShowCustomPopup(new Popup($"Ошибка обновления", PopupType.Error, 3000));
            Logger.LogError($"{e.Message} \n{e.InnerException}");
        }
        
        _ = Task.Run(async () =>
        {
            await Task.Delay(10000);
            UpdateDownloadingStatus(DownloadStatus.NotFound);
        });
    }

    /// <summary>
    /// Этот метод вызывается при активации страницы.
    /// </summary>
    public override void OnActivated(INavigationService navigationService)
    {
        base.OnActivated(navigationService);
        Logger.LogInformation("MainMenuPageViewModel activated");
    }

    /// <summary>
    /// Скопировать директорию с БД в директорию загрузок.
    /// </summary>
    private async Task CopyDataBaseDirectoryToDownloads()
    {
        Logger.LogInformation("Вызвано копирование");
        await _fileExplorer.CopyDataBaseDirectoryToDownloadsAsync();

        _popupService.ShowSuccess("Каталог успешно скопирован!");
        Logger.LogInformation("Каталог успешно скопирован!");
    }
    
    /// <summary>
    /// Закрыть смену.
    /// </summary>
    private async Task ShiftClose()
    {
        var openShift = await _shiftService.GetOpenedShiftOrDefaultAsync();
        var shiftNumber = openShift != null ? openShift.ShiftKey : 0;
        
        var confirmPage = new ConfirmationPageViewModel(
            Logger,
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

                    if (_configurationService.SettingsFromPosOffice.MainSettings.Incass.Auto)
                        await EncashmentAsync();
                    
                    Navigation!.NavigateTo<OpenShiftPageViewModel>();
                }
                catch (Exception e)
                {
                    Logger.LogInformation(e.Message);
                    Navigation!.NavigateTo<OpenShiftPageViewModel>();
                }
            },
            () =>
            {
                Navigation!.NavigateTo<MainMenuPageViewModel>();
            });
        
        Navigation!.NavigateToInstancePage(confirmPage);
    }
    
    /// <summary>
    /// Напечатать промежуточный отчёт за смену.
    /// </summary>
    /// <param name="arg">Токен отмены.</param>
    private async Task PrintInterimReport(CancellationToken arg)
    {
        try
        {
            var openShift = await _shiftService.GetOpenedShiftOrDefaultAsync();

            if (openShift == null)
            {
                _popupService.ShowError("Ни одна смена не открыта.");
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
                IssuerNumber = issuerNumber ?? "",
                TerminalNumber = terminalNumber ?? "",
                Shift = openShift,
                SalesList = sales,
                OperatorName = !string.IsNullOrEmpty(operatorName) ? operatorName : "undefined"
            };

            await _receiptPrintService.PrintShiftReportAsync(reportData);
        }
        catch (Exception e)
        {
            _popupService.ShowError(e.Message);
            Logger.LogError($"Ошибка печати промежуточного отчёта\n{e.Message}\n{e.InnerException}");
        }
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
        Navigation!.NavigateToInstancePage(authPage);
    }

    /// <summary>
    /// Запустить инкассацию.
    /// </summary>
    private async Task EncashmentAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();
            
            await _configurationUpdatingService.UpdateSettingsFromPosTms();

            stopwatch.Stop();
            
            if (_configurationService.SettingsFromPosOffice.MainSettings.Incass.Auto)
                await PrintInterimReport(cancellationToken);
            
            stopwatch.Start();
                    
            await _encashmentService.EncashmentAsync();
            
            stopwatch.Stop();
    
            _popupService.ShowSuccess($"Инкассация выполнена за {stopwatch.Elapsed}");
        }
        catch (Exception e)
        {
            _popupService.ShowError(e.Message);
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
    
    /// <summary>
    /// Обновить статус скачивания обновлений.
    /// </summary>
    /// <param name="downloadingStatus">Статус скачивания.</param>
    private void UpdateDownloadingStatus(DownloadStatus downloadingStatus)
    {
        var status = new Status { Type = StatusType.UpdatePatch };
        
        if (downloadingStatus == DownloadStatus.NotFound)
        {
            _statusNotifierService.RemoveStatusByType(status.Type);
        }
        else
        {
            status.IconName = downloadingStatus switch
            {
                DownloadStatus.InProcess => DownloadIconName,
                DownloadStatus.Aborted => AbortedIconName,
                DownloadStatus.Completed => CompletedIconName,
                _ => status.IconName
            };

            _statusNotifierService.AddOrChangeStatus(status);    
        }
        
        _statusNotifierService.Notify();
    }
}