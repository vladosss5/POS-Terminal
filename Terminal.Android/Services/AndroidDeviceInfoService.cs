using Microsoft.Maui.Devices;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Entities.Models;
using Terminal.Core.Interfaces;
using DevicePlatform = Terminal.Core.Enums.DevicePlatform;

namespace Terminal.Android.Services;

/// <summary>
/// Android реализация сервиса по работе с информацией об устройстве.
/// </summary>
public class AndroidDeviceInfoService : IDeviceInfoService
{
    /// <inheritdoc/>
    public DeviceInfoModel DeviceInformation { get; init; }

    /// <summary>
    /// Конструктор.
    /// </summary>
    public AndroidDeviceInfoService()
    {
        DeviceInformation = new DeviceInfoModel
        {
            Platform = DevicePlatform.Android,
            Manufacturer = DeviceInfo.Current.Manufacturer,
            Model = DeviceInfo.Current.Model,
            Name = DeviceInfo.Current.Name
        };
    }
}