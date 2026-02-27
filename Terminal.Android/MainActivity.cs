using Android.App;
using Android.Content.PM;
using Android.OS;
using Avalonia;
using Avalonia.Android;
using Microsoft.Extensions.DependencyInjection;
using Terminal.Android.Extensions;
using Terminal.Extensions;

namespace Terminal.Android;

[Activity(
    Label = "Terminal.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        var services = new ServiceCollection();
        
        services.AddLogger();
        services.AddCommonServices();
        services.AddAndroidServices();
        services.AddDataContext();
        
        App.Services = services.BuildServiceProvider();
        
        base.OnCreate(savedInstanceState);
    }
    
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    { 
        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }
}