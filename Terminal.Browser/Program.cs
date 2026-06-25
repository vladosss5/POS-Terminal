using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Terminal;
using Terminal.Extensions;

internal sealed partial class Program
{
    private static IHost? _host;
    
    public static async Task Main(string[] args)
    {
        _host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddLogger();
                services.AddCommonServices();
                services.AddDataContext();
                services.AddTmsClient();
            })
            .Build();

        await _host.StartAsync();
        
        App.Services = _host.Services;

        await BuildAvaloniaApp()
            .WithInterFont()
            .StartBrowserAppAsync("out");
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}