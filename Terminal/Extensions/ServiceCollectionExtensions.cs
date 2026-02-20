using Microsoft.Extensions.DependencyInjection;
using Terminal.Application.Implementations.Builders;
using Terminal.Application.Interfaces.Builders;
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
        collection.AddTransient<MainViewModel>();

        collection.AddTransient<MainMenuPageView>();
        collection.AddTransient<MainMenuPageViewModel>();

        collection.AddTransient<RefuelingByCardPageViewModel>();
        collection.AddTransient<RefuelingByCardPageView>();

        collection.AddScoped<IRefuelingProcessBuilder, RefuelingProcessBuilder>();
    }
}