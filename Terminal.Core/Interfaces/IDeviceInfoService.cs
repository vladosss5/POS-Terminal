using Terminal.Core.Entities.Models;

namespace Terminal.Core.Interfaces;

/// <summary>
/// Сервис информации об устройстве.
/// </summary>
public interface IDeviceInfoService
{
    /// <summary>
    /// Информация об устройстве.
    /// </summary>
    public DeviceInfoModel DeviceInformation { get; init; }
}