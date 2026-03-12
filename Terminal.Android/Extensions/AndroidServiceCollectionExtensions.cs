using Microsoft.Extensions.DependencyInjection;
using Terminal.Android.Services;
using Terminal.Android.Services.Sunyard.SunyardCardReader;
using Terminal.Android.Services.Sunyard.SunyardPrinter;
using Terminal.Application.Interfaces.Services;

namespace Terminal.Android.Extensions;

/// <summary>
/// Расширение коллекции сервисов для Android.
/// </summary>
public static class AndroidServiceCollectionExtensions
{
    /// <param name="collection"></param>
    extension(IServiceCollection collection)
    {
        /// <summary>
        /// Регистрация сервисов для Android.
        /// </summary>
        public void AddAndroidServices()
        {
            collection.AddSingleton(global::Android.App.Application.Context);
            collection.AddScoped<IFileExplorer, AndroidFileExplorer>();
            collection.AddTransient<IReceiptPrintService, AndroidPrintService>();
        }

        /// <summary>
        /// Регистрация сервисов для терминалов Sunyard (alps)
        /// </summary>
        public void AddServicesForSunyard()
        {
            collection.AddTransient<IReceiptPrintService, SunyardPrintService>();
            collection.AddTransient<ICardReaderService, SunyardCardReaderService>();
        }
    }
}