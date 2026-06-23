using System;
using Android.App;
using Android.Runtime;
using Avalonia;
using Avalonia.Android;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Devices;
using Terminal.Android.Extensions;
using Terminal.Extensions;

namespace Terminal.Android;

[Application]
public class AndroidApp : AvaloniaAndroidApplication<App>
{
    public AndroidApp(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
    {
    }
    
    public override void OnCreate()
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

        base.OnCreate();
    }
    
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base
            .CustomizeAppBuilder(builder)
            .WithInterFont();
    }
}