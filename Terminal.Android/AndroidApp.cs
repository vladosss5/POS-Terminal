using System;
using Android.App;
using Android.Runtime;
using Avalonia.Android;
using Microsoft.Extensions.Hosting;
using Microsoft.Maui.Devices;
using Terminal.Android.Extensions;
using Terminal.Extensions;

namespace Terminal.Android;

[Application]
public class AndroidApp : AvaloniaAndroidApplication<App>
{
    private IHost? _host;
    
    public AndroidApp(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
    {
    }
    
    public override void OnCreate()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddLogger();
                services.AddCommonServices();
                services.AddAndroidServices();
                services.AddDataContext();
                services.AddTmsClient();

                var deviceManufacturer = DeviceInfo.Current.Manufacturer;
                if (deviceManufacturer == "alps")
                    services.AddServicesForSunyard();
            })
            .Build();

        _host.StartAsync().GetAwaiter().GetResult();

        App.Services = _host.Services;

        base.OnCreate();
    }
    
    public override void OnTerminate()
    {
        _host?.StopAsync().GetAwaiter().GetResult();
        _host?.Dispose();
        base.OnTerminate();
    }
}