using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Microsoft.Extensions.DependencyInjection;
using Terminal;
using Terminal.Extensions;

internal sealed partial class Program
{
    public static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        
        services.AddLogger();
        services.AddCommonServices();
        services.AddDataContext();
        services.AddTmsClient();

        App.Services = services.BuildServiceProvider();

        await BuildAvaloniaApp()
            .WithInterFont()
            .StartBrowserAppAsync("out");
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}