using Android.App;
using Android.Content.PM;
using Android.OS;
using Avalonia;
using Avalonia.Android;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Devices;
using Terminal.Android.Extensions;
using Terminal.Extensions;

namespace Terminal.Android;

[Activity(
    Label = "SncTerminal",
    Theme = "@style/Theme.AppCompat.DayNight.NoActionBar",
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
        services.AddTmsClient();

        var deviceManufacturer = DeviceInfo.Current.Manufacturer;
        if (deviceManufacturer == "alps")
            services.AddServicesForSunyard();
        
        App.Services = services.BuildServiceProvider();
        
        base.OnCreate(savedInstanceState);
    }
    
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    { 
        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }
}