using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Microsoft.Extensions.Hosting;
using Terminal.Extensions;

namespace Terminal.Browser;

internal static partial class Program
{
    private static IHost? _host;
    
    public static async Task Main(string[] args)
    {
        _host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
            {
                services.AddLogger();
                services.AddCommonServices();
                services.AddDataContext();
            })
            .Build();

        await _host.StartAsync();
        
        App.Services = _host.Services;

        await BuildAvaloniaApp()
            .WithInterFont()
            .StartBrowserAppAsync("out");
    }

    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}