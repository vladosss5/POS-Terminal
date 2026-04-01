using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Terminal.Application.Implementations.Builders;
using Terminal.Application.Implementations.DbEntitiesServices;
using Terminal.Application.Implementations.Mappers;
using Terminal.Application.Implementations.Services;
using Terminal.Application.Interfaces.Builders;
using Terminal.Application.Interfaces.DbEntitiesServices;
using Terminal.Application.Interfaces.Mappers;
using Terminal.Application.Interfaces.Services;
using Terminal.Converters;
using Terminal.Data.Context;
using Terminal.Services;
using Terminal.Services.NavigationService;
using Terminal.ViewModels;
using Terminal.ViewModels.Pages;

namespace Terminal.IntegrationTests;

public static class TestServiceProvider
{
    public static void RegisterCommonServices(this IServiceCollection collection)
    {
        collection.AddLogging(builder =>
        {
            builder
                .AddConsole()
                .AddDebug();
        });
        
        collection.AddSingleton<IConfigurationService, ConfigurationService>();
        
        // ViewModels
        collection.AddTransient<MainViewModel>();
        collection.AddTransient<MainMenuPageViewModel>();
        collection.AddTransient<SaleProcessPageViewModel>();
        collection.AddTransient<PrintingReceiptPageViewModel>();
        collection.AddSingleton<SettingsMenuPageViewModel>();
        collection.AddTransient<PaymentTypesSettingsPageViewModel>();
        collection.AddTransient<OpenShiftPageViewModel>();
        collection.AddTransient<SettingsShiftOpeningPageViewModel>();
        collection.AddTransient<AdminLoginPageViewModel>();
        collection.AddTransient<SettingsPrintPageViewModel>();
        
        // Конвертеры
        collection.AddSingleton<EnumFriendlyNameConverter>();
        collection.AddSingleton<PathToImageConverter>();
        
        // Мапперы
        collection.AddSingleton<ISettingPaymentTypeMapper, SettingPaymentTypeMapper>();
        collection.AddSingleton<ISalesReceiptMappingService, SalesReceiptMappingService>();
        
        // Сервисы логики
        collection.AddTransient<ISellingBuilder, SellingBuilder>();
        
        collection.AddSingleton(_ => new Mock<INavigationService>().Object);
        collection.AddSingleton(new Mock<INavigationService>());
        
        collection.AddScoped<IFileReader, FileReader>();
        collection.AddScoped<ISqlExecutor, SqlExecutor>();
        collection.AddTransient<ICardReaderService, CommonCardReaderService>();
        collection.AddSingleton<IHashService, HashService>();
        collection.AddSingleton<IAuthService, AuthService>();
        collection.AddTransient<IShiftService, ShiftService>();
    }
    
    /// <summary>
    /// Регистрация контекста БД.
    /// </summary>
    public static void AddDataContext(this IServiceCollection collection)
    {
        var dbPath = DataContext.GetTestingDbPath();
        var dir = Path.GetDirectoryName(dbPath);
        
        if (dir != null)
            Directory.CreateDirectory(dir);
        
        collection.AddDbContextFactory<DataContext>(options =>
        {
            options.UseSqlite($"Data Source={dbPath}");
        });
    }
}