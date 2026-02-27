using System;
using Avalonia;
using HotAvalonia;
using Microsoft.Extensions.DependencyInjection;
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
        var services = new ServiceCollection();
        
        services.AddLogger();
        services.AddCommonServices();
        services.AddDesktopServices();
        services.AddDataContext();
        
        App.Services = services.BuildServiceProvider();
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