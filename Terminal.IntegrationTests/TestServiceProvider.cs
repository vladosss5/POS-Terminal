using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Terminal.Application.Builders;
using Terminal.Application.Interfaces.Builders;
using Terminal.Application.Interfaces.Mappers;
using Terminal.Application.Interfaces.Services;
using Terminal.Application.Mappers;
using Terminal.Application.Services;
using Terminal.Converters;
using Terminal.Core.Interfaces;
using Terminal.Persistence.MainDB;
using Terminal.Persistence.ParamDB;
using Terminal.Services;
using Terminal.Services.MessageBoxService;
using Terminal.Services.NavigationService;
using Terminal.ViewModels;
using Terminal.ViewModels.Pages;

namespace Terminal.IntegrationTests;

public static class TestServiceProvider
{
    public static void RegisterCommonServices(this IServiceCollection collection,
        Mock<INavigationService> navigationMock,
        Mock<IMessageBoxService> messageMock,
        Mock<ICardReaderService> cardReaderMock,
        Mock<IReceiptPrintService> receiptPrintMock)
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
        collection.AddSingleton<SettingsPageViewModel>();
        collection.AddTransient<OpenShiftPageViewModel>();
        collection.AddTransient<AdminLoginPageViewModel>();
        
        // Конвертеры
        collection.AddSingleton<EnumFriendlyNameConverter>();
        collection.AddSingleton<PathToImageConverter>();
        
        // Мапперы
        collection.AddSingleton<ISettingPaymentTypeMapper, SettingPaymentTypeMapper>();
        collection.AddSingleton<ISalesReceiptMappingService, SalesReceiptMappingService>();
        
        // Сервисы логики
        collection.AddTransient<ISellingBuilder, SellingBuilder>();
        
        collection.AddSingleton(messageMock.Object);
        collection.AddSingleton(messageMock);

        collection.AddSingleton(navigationMock.Object);
        collection.AddSingleton(navigationMock);
        
        collection.AddSingleton(cardReaderMock.Object);
        collection.AddSingleton(cardReaderMock);

        collection.AddSingleton(receiptPrintMock.Object);
        collection.AddSingleton(receiptPrintMock);
        
        collection.AddScoped<IFileReader, FileReader>();
        collection.AddScoped<ISqlExecutor, SqlExecutorMainDb>();
        collection.AddScoped<ICryptographyService, CryptographyService>();
        collection.AddScoped<IAuthService, AuthService>();
        collection.AddTransient<IShiftService, ShiftService>();
    }
    
    /// <summary>
    /// Регистрация контекстов БД.
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

        
        var paramDbPath = ParamDbContext.GetDbPathForIntegrationTests();
        var paramDir = Path.GetDirectoryName(paramDbPath);

        if (paramDir != null)
            Directory.CreateDirectory(paramDir);
        
        collection.AddDbContextFactory<ParamDbContext>(options =>
        {
            options.UseSqlite($"Data Source={paramDbPath}");
        });
    }
}