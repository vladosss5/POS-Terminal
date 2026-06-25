using System;
using Avalonia;
using HotAvalonia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Terminal.Desktop.Extensions;
using Terminal.Extensions;

namespace Terminal.Desktop;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddLogger();
                services.AddCommonServices();
                services.AddDesktopServices();
                services.AddDataContext();
                services.AddTmsClient();
            })
            .Build();
    
        _ = host.RunAsync();
    
        App.Services = host.Services;
    
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseHotReload();
}