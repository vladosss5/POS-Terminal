using Terminal.Application.Interfaces.Services;
using Terminal.Core.Models;
using DevicePlatform = Terminal.Core.Enums.DevicePlatform;

namespace Terminal.Desktop.Services;

public class DesktopDeviceInfoService : IDeviceInfoService
{
    /// <inheritdoc/>
    public DeviceInfoModel DeviceInformation { get; init; }

    /// <summary>
    /// Конструктор.
    /// </summary>
    public DesktopDeviceInfoService()
    {
        DeviceInformation = new DeviceInfoModel
        {
            Platform = DevicePlatform.Desktop,
            Manufacturer = "Unknown",
            Model = "Unknown",
            Name = "Unknown"
        };
    }
}