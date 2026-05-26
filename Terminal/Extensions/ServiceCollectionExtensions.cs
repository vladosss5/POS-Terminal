using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Terminal.Application.Implementations.Builders;
using Terminal.Application.Implementations.DbEntitiesServices;
using Terminal.Application.Implementations.Mappers;
using Terminal.Application.Implementations.Services;
using Terminal.Application.Interfaces.Builders;
using Terminal.Application.Interfaces.DbEntitiesServices;
using Terminal.Application.Interfaces.Mappers;
using Terminal.Application.Interfaces.Services;
using Terminal.Converters;
using Terminal.Core.Enums;
using Terminal.Persistence.EventDB;
using Terminal.Persistence.MainDB;
using Terminal.Persistence.ParamDB;
using Terminal.Persistence.TmsClient;
using Terminal.Services;
using Terminal.Services.AuthPageFactory;
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
        collection.AddTransient<SaleProcessPageViewModel>();
        collection.AddTransient<PrintingReceiptPageViewModel>();
        collection.AddSingleton<SettingsPageViewModel>();
        collection.AddTransient<OpenShiftPageViewModel>();
        collection.AddTransient<AdminLoginPageViewModel>();
        collection.AddTransient<InitialSetupPageViewModel>();
        collection.AddSingleton<ShiftReportPageViewModel>();
        collection.AddTransient<ResourcePageViewModel>();
        collection.AddTransient<AuthOperatorPageViewModel>();
        
        // Конвертеры
        collection.AddSingleton<EnumFriendlyNameConverter>();
        collection.AddSingleton<PathToImageConverter>();
        
        // Мапперы
        collection.AddSingleton<ISettingPaymentTypeMapper, SettingPaymentTypeMapper>();
        collection.AddSingleton<ISalesReceiptMappingService, SalesReceiptMappingService>();
        
        // Сервисы логики
        collection.AddTransient<IMessageBoxService, MessageBoxService>();
        collection.AddTransient<ISellingBuilder, SellingBuilder>();
        collection.AddSingleton<INavigationService, NavigationService>();
        collection.AddScoped<IFileReader, FileReader>();
        collection.AddScoped<ISqlExecutor, SqlExecutor>();
        collection.AddTransient<ICardReaderService, CommonCardReaderService>();
        collection.AddSingleton<ICryptographyService, CryptographyService>();
        collection.AddSingleton<IAuthService, AuthService>();
        collection.AddTransient<IShiftService, ShiftService>();
        collection.AddSingleton<IAuthPageFactory, AuthPageFactory>();
        collection.AddSingleton<IParameterService, ParameterService>();
        collection.AddTransient<IEncashmentService, EncashmentService>();
        collection.AddSingleton<IMainDbService, MainDbService>();
        collection.AddSingleton<IEventDbService, EventDbService>();
    }

    /// <summary>
    /// Регистрация TMS клиента.
    /// </summary>
    public static void AddTmsClient(this IServiceCollection collection)
    {
        collection.AddSingleton<ITmsClient>(sp =>
        {
            var parameterService = sp.GetRequiredService<IParameterService>();
        
            var ipTms = parameterService.GetValueAsync(AppParameter.TmsIp).GetAwaiter().GetResult();
            var portTms = parameterService.GetValueAsync(AppParameter.TmsPort).GetAwaiter().GetResult();
            var addressBase = $"http://{ipTms}:{portTms}/";
        
            return new TmsClient(addressBase);
        });
    }
    
    /// <summary>
    /// Регистрация контекста БД.
    /// </summary>
    public static void AddDataContext(this IServiceCollection collection)
    {
        // Main DataContext
        var dbPath = DataContext.GetDefaultDbPath();
        var dir = Path.GetDirectoryName(dbPath);
        
        if (dir != null)
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
        
        if (paramDbDir != null)
            Directory.CreateDirectory(paramDbDir);

        collection.AddDbContextFactory<ParamDbContext>(options =>
        {
            options.UseSqlite($"Data Source={paramDbPath}");
        });
        
        // EventDbContext
        var eventDbPath = EventDbContext.GetDefaultDbPath();
        var eventDbDir = Path.GetDirectoryName(eventDbPath);
        
        if (eventDbDir != null)
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