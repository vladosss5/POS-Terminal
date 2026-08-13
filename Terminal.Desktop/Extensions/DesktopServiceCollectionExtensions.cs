using MainHelpers.Logger;
using Microsoft.Extensions.DependencyInjection;
using Terminal.Application.Interfaces.Services;
using Terminal.Application.Services;
using Terminal.Core.Interfaces;
using Terminal.Desktop.Services;

namespace Terminal.Desktop.Extensions;

/// <summary>
/// Расширение коллекции сервисов для Android.
/// </summary>
public static class DesktopServiceCollectionExtensions
{
    /// <summary>
    /// Регистрация сервисов для Android.
    /// </summary>
    public static void AddDesktopServices(this IServiceCollection collection)
    {
        collection.AddScoped<IFileExplorer, FileExplorer>();
        collection.AddSingleton<IReceiptPrintService, DesktopPrintService>();
        collection.AddSingleton<IDeviceInfoService, DesktopDeviceInfoService>();
        collection.AddSingleton<IUpdateInstallerService, DesktopUpdateInstallerService>();
        collection.AddSingleton<IDiscountingLibraryService, DesktopDiscountingLibraryService>();
        collection.AddSingleton<IXmlResourceProvider, DesktopXmlResourceProvider>();
        collection.AddSingleton<ISoundService, DesktopSoundService>();
    }
    
    /// <summary>
    /// Регистрация SNC логера.
    /// </summary>
    public static void AddDesktopLogger(this IServiceCollection services)
    {
        var logger = new LoggerClass("Terminal", LogSaveType.Full, LogType.Txt, "logs");
        LoggerClass.StaticFlags = LoggerFlagsType.Console;
    
        services.AddSingleton(logger);
    }
}