
using System;
using System.IO;
using MainHelpers.Logger;
using Microsoft.Extensions.DependencyInjection;
using Terminal.Android.Services;
using Terminal.Android.Services.Sunyard.SunyardCardReader;
using Terminal.Android.Services.Sunyard.SunyardPrinter;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Interfaces;

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
            collection.AddSingleton<IDeviceInfoService, AndroidDeviceInfoService>();
            collection.AddSingleton<IUpdateInstallerService, AndroidUpdateInstallerService>();
            collection.AddSingleton<IXmlResourceProvider, AndroidXmlResourceProvider>();
            collection.AddSingleton<IDiscountingLibraryService, AndroidDiscountingLibraryService>();
            collection.AddSingleton<ISoundService, AndroidSoundService>();
        }

        /// <summary>
        /// Регистрация сервисов для терминалов Sunyard (alps)
        /// </summary>
        public void AddServicesForSunyard()
        {
            collection.AddTransient<IReceiptPrintService, SunyardPrintService>();
            collection.AddTransient<ICardReaderService, SunyardCardReaderService>();
        }
        
        /// <summary>
        /// Регистрация сервиса логирования.
        /// </summary>
        public void AddAndroidLogger()
        {
            var documentsPath = global::Android.OS.Environment.GetExternalStoragePublicDirectory(
                global::Android.OS.Environment.DirectoryDocuments)?.AbsolutePath;

            if (string.IsNullOrEmpty(documentsPath))
                documentsPath = "/storage/emulated/0/Documents";

            var logsPath = Path.Combine(documentsPath, "TerminalLogs");
    
            if (!Directory.Exists(logsPath))
                Directory.CreateDirectory(logsPath);
            
            var logger = new LoggerClass("Terminal", LogSaveType.Full, LogType.Txt, logsPath);
            LoggerClass.StaticFlags = LoggerFlagsType.Console;
            logger.Open();
            collection.AddSingleton(logger);
        }
    }
}