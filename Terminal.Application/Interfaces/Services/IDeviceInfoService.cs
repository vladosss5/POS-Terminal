using Terminal.Core.Models;

namespace Terminal.Application.Interfaces.Services;

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