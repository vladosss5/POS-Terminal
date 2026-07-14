using Terminal.Core.Enums;

namespace Terminal.Core.Entities.Models;

/// <summary>
/// Информация об устройстве на котором запущено приложение.
/// </summary>
public class DeviceInfoModel
{
    /// <summary>
    /// Платформа.
    /// </summary>
    public DevicePlatform Platform { get; set; }
    
    /// <summary>
    /// Производитель.
    /// </summary>
    public string? Manufacturer { get; set; }
    
    /// <summary>
    /// Модель.
    /// </summary>
    public string? Model { get; set; }
    
    /// <summary>
    /// Наименование.
    /// </summary>
    public string? Name { get; set; }
}