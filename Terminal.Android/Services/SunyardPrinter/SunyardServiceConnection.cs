using System;
using Android.Content;
using Android.OS;
using Com.Sunyard.Api;

namespace Terminal.Android.Services.SunyardPrinter;

public class SunyardServiceConnection : Java.Lang.Object, IServiceConnection
{
    private readonly Action<IDeviceService> _onConnected;
    private readonly Action _onDisconnected;

    public SunyardServiceConnection(Action<IDeviceService> onConnected, Action onDisconnected)
    {
        _onConnected = onConnected;
        _onDisconnected = onDisconnected;
    }

    public void OnServiceConnected(ComponentName name, IBinder service)
    {
        var deviceService = IDeviceService.Stub.AsInterface(service);
        _onConnected?.Invoke(deviceService);
    }

    public void OnServiceDisconnected(ComponentName name)
    {
        _onDisconnected?.Invoke();
    }
}