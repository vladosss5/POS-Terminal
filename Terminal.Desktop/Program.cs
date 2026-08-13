using System;
using Avalonia;
using HotAvalonia;
using Microsoft.Extensions.Hosting;
using Terminal.Desktop.Extensions;
using Terminal.Extensions;

namespace Terminal.Desktop;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
            {
                services.AddLogger();
                services.AddDesktopLogger();
                services.AddCommonServices();
                services.AddDesktopServices();
                services.AddDataContext();
            })
            .Build();
    
        _ = host.RunAsync();
    
        App.Services = host.Services;
    
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }
    
    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseHotReload();
}