using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Terminal.Application.Implementations.Builders;
using Terminal.Application.Implementations.Services;
using Terminal.Application.Interfaces.Builders;
using Terminal.Application.Interfaces.Services;
using Terminal.Application.UseCases;
using Terminal.Converters;
using Terminal.Data.Context;
using Terminal.ViewModels;
using Terminal.ViewModels.NavigationService;
using Terminal.ViewModels.Pages;
using Terminal.Views.Pages;

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
        // View и ViewModel
        collection.AddTransient<MainViewModel>();

        collection.AddTransient<MainMenuPageView>();
        collection.AddTransient<MainMenuPageViewModel>();

        collection.AddTransient<RefuelingByCardPageViewModel>();
        collection.AddTransient<RefuelingByCardPageView>();
        
        // Конвертеры
        collection.AddSingleton<EnumFriendlyNameConverter>();
        
        // Сервисы логики
        collection.AddScoped<IRefuelingProcessBuilder, RefuelingProcessBuilder>();
        collection.AddSingleton<INavigationService, NavigationService>();
        collection.AddScoped<IFileReader, FileReader>();
        collection.AddScoped<ISqlExecutor, SqlExecutor>();
        
// #if !ANDROID
//         collection.AddScoped<IFileExplorer, FileExplorer>();
// #endif
        
        // UseCases
        collection.AddScoped<ExecuteSqlScriptsHandler>();
    }

    public static void AddDataContext(this IServiceCollection collection)
    {
        var dbPath = DataContext.GetDefaultDbPath();
        
        string? dir = Path.GetDirectoryName(dbPath);
        if (dir != null)
        {
            Directory.CreateDirectory(dir);
        }
        
        collection.AddDbContextFactory<DataContext>(options =>
        {
            options.UseSqlite($"Data Source={dbPath}");
            
#if DEBUG
            options
                .EnableSensitiveDataLogging()     // показывает параметры запросов (осторожно в прод!)
                .EnableDetailedErrors()
                .LogTo(Console.WriteLine, LogLevel.Information);  // или LogLevel.Debug
#endif
        });
    }

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