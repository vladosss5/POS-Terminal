using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Background;
using Terminal.Application.Interfaces.Mappers;
using Terminal.Application.Interfaces.Services;
using Terminal.Application.Mappers;
using Terminal.Application.Services;
using Terminal.Converters;
using Terminal.Core.Interfaces;
using Terminal.Core.IRepositories;
using Terminal.Persistence.EventDB;
using Terminal.Persistence.MainDB;
using Terminal.Persistence.ParamDB;
using Terminal.Persistence.Repositories;
using Terminal.Persistence.TmsClient;
using Terminal.Services;
using Terminal.Services.AuthPageFactory;
using Terminal.Services.Mappers.ResourceCodeMapping;
using Terminal.Services.MessageBoxService;
using Terminal.Services.NavigationService;
using Terminal.ViewModels;
using Terminal.ViewModels.Pages;

namespace Terminal.Extensions;

/// <summary>
/// Настройка DI.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Регистрация платформонезависимых сервисов, view и VM в DI.
    /// </summary>
    /// <param name="collection">Дополняемая коллекция сервисов.</param>
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddSingleton<IConfigurationService, ConfigurationService>();
        
        collection.AddHttpClient();
        
        // ViewModels
        collection.AddTransient<MainViewModel>();
        collection.AddTransient<MainMenuPageViewModel>();
        collection.AddTransient<PrintingReceiptPageViewModel>();
        collection.AddSingleton<SettingsPageViewModel>();
        collection.AddTransient<OpenShiftPageViewModel>();
        collection.AddTransient<AdminLoginPageViewModel>();
        collection.AddTransient<InitialSetupPageViewModel>();
        collection.AddSingleton<ShiftReportPageViewModel>();
        collection.AddTransient<ResourcePageViewModel>();
        collection.AddTransient<AuthOperatorPageViewModel>();
        collection.AddTransient<SellingProcessPageViewModel>();
        
        // Конвертеры
        collection.AddSingleton<EnumFriendlyNameConverter>();
        collection.AddSingleton<PathToImageConverter>();
        
        // Мапперы
        collection.AddSingleton<ISettingPaymentTypeMapper, SettingPaymentTypeMapper>();
        collection.AddSingleton<ISellingMappingService, SellingMappingService>();
        collection.AddSingleton<IResourceCodeMapper, ResourceCodeMapper>();
        
        // Сервисы логики
        collection.AddSingleton<ITmsClient, TmsClient>();
        collection.AddTransient<IMessageBoxService, MessageBoxService>();
        collection.AddSingleton<INavigationService, NavigationService>();
        collection.AddScoped<IFileReader, FileReader>();
        collection.AddScoped<ISqlExecutor, SqlExecutorMainDb>();
        collection.AddTransient<ICardReaderService, CommonCardReaderService>();
        collection.AddSingleton<ICryptographyService, CryptographyService>();
        collection.AddSingleton<IAuthService, AuthService>();
        collection.AddTransient<IShiftService, ShiftService>();
        collection.AddSingleton<IAuthPageFactory, AuthPageFactory>();
        collection.AddSingleton<IParameterService, ParameterService>();
        collection.AddTransient<IEncashmentService, EncashmentService>();
        collection.AddTransient<ITmsService, TmsService>();
        collection.AddSingleton<IConfigurationUpdatingService, ConfigurationUpdatingService>();
        collection.AddSingleton<IStatusNotifierService, StatusNotifierService>();
        collection.AddSingleton<IStepNotifierService, StepNotifierService>();
        collection.AddTransient<ISalesProcessService, SalesProcessService>();
        collection.AddSingleton<IDiscountingMethods, DiscountingMethods>();
        
        // Репозитории.
        collection.AddTransient<IGenericRepository, GenericRepository>();
        collection.AddTransient<IParamRepository, ParamRepository>();
        collection.AddTransient<ISellingRepository, SellingRepository>();
        collection.AddTransient<ISettingRepository, SettingRepository>();
        collection.AddTransient<IShiftRepository, ShiftRepository>();
        collection.AddTransient<IUserRepository, UserRepository>();
        collection.AddSingleton<IResourceCodeRepository, ResourceCodeRepository>();
        
        // Фоновые сервисы
        collection.AddSingleton<IUpgradeBackgroundService, UpgradeBackgroundService>();
    }
    
    /// <summary>
    /// Регистрация контекста БД.
    /// </summary>
    public static void AddDataContext(this IServiceCollection collection)
    {
        // Main DataContext
        var dbPath = DataContext.GetDefaultDbPath();
        var dir = Path.GetDirectoryName(dbPath);
        
        if (!Path.Exists(dir) && dir != null)
            Directory.CreateDirectory(dir);
        
        collection.AddDbContextFactory<DataContext>(options =>
        {
            options.UseSqlite($"Data Source={dbPath}");
            
#if DEBUG
            options
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
                .LogTo(Console.WriteLine, LogLevel.Information);
#endif
        });

        // ParamDbContext
        var paramDbPath = ParamDbContext.GetDefaultDbPath();
        var paramDbDir = Path.GetDirectoryName(paramDbPath);
        
        if (!Path.Exists(paramDbDir) && paramDbDir != null)
            Directory.CreateDirectory(paramDbDir);

        collection.AddDbContextFactory<ParamDbContext>(options =>
        {
            options.UseSqlite($"Data Source={paramDbPath}");
        });
        
        // EventDbContext
        var eventDbPath = EventDbContext.GetDefaultDbPath();
        var eventDbDir = Path.GetDirectoryName(eventDbPath);
        
        if (!Path.Exists(eventDbDir) && eventDbDir != null)
            Directory.CreateDirectory(eventDbDir);

        collection.AddDbContextFactory<EventDbContext>(options =>
        {
            options.UseSqlite($"Data Source={eventDbPath}");
        });
    }

    /// <summary>
    /// Регистрация логгера.
    /// </summary>
    public static void AddLogger(this IServiceCollection collection)
    {
        collection.AddLogging(builder =>
        {
            builder
                .AddConsole()
                .AddDebug();

#if DEBUG
            builder.SetMinimumLevel(LogLevel.Debug);
#else
        builder.SetMinimumLevel(LogLevel.Information);
#endif
        });
    }
}