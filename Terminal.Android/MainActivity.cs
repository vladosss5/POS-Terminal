using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Avalonia;
using Avalonia.Android;
using Com.Sunyard.Api.System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Devices;
using Terminal.Android.Extensions;
using Terminal.Android.Services.DisplayMode;
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