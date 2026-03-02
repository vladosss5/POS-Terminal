using Android.App;
using Android.Content;
using Microsoft.Extensions.DependencyInjection;
using Terminal.Android.Services;
using Terminal.Android.Services.SunyardPrinter;
using Terminal.Application.Interfaces.Services;

namespace Terminal.Android.Extensions;

/// <summary>
/// Расширение коллекции сервисов для Android.
/// </summary>
public static class AndroidServiceCollectionExtensions
{
    /// <summary>
    /// Регистрация сервисов для Android.
    /// </summary>
    public static void AddAndroidServices(this IServiceCollection collection)
    {
        collection.AddSingleton(global::Android.App.Application.Context);
        collection.AddScoped<IFileExplorer, AndroidFileExplorer>();
        collection.AddSingleton<IPrintService, SunyardPrintService>();
    }
}