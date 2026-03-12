using System;
using Android.Content;
using Android.OS;
using Com.Sunyard.Api;

namespace Terminal.Android.Services.Sunyard;

/// <summary>
/// Реализация IServiceConnection для подключения к системному сервису Sunyard.
/// Обрабатывает события подключения и отключения от сервиса, предоставляя
/// объект IDeviceService через callback.
/// </summary>
public class SunyardServiceConnection : Java.Lang.Object, IServiceConnection
{
    private readonly Action<IDeviceService> _onConnected;
    private readonly Action _onDisconnected;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public SunyardServiceConnection(Action<IDeviceService> onConnected, Action onDisconnected)
    {
        _onConnected = onConnected;
        _onDisconnected = onDisconnected;
    }

    /// <summary>
    /// Вызывается системой Android при успешном подключении к сервису.
    /// Преобразует IBinder в объект IDeviceService через стаб-класс.
    /// </summary>
    /// <param name="name">Имя компонента сервиса.</param>
    /// <param name="service">IBinder для взаимодействия с сервисом.</param>
    public void OnServiceConnected(ComponentName name, IBinder service)
    {
        var deviceService = IDeviceService.Stub.AsInterface(service);
        _onConnected?.Invoke(deviceService);
    }
    
    /// <summary>
    /// Вызывается системой Android при отключении от сервиса
    /// (например, если сервис был остановлен или аварийно завершён).
    /// </summary>
    /// <param name="name">Имя компонента сервиса.</param>
    public void OnServiceDisconnected(ComponentName name)
    {
        _onDisconnected?.Invoke();
    }
}