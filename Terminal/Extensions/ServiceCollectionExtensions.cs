using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Terminal.Application.Implementations.Builders;
using Terminal.Application.Interfaces.Builders;
using Terminal.Converters;
using Terminal.Data.Context;
using Terminal.ViewModels;
using Terminal.ViewModels.Pages;
using Terminal.Views.Pages;

namespace Terminal.Extensions;

/// <summary>
/// Настройка DI.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Регистрация сервисов, view и VM в DI.
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
        });
    }
}