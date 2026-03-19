using System.Threading.Tasks;

namespace Terminal.Android.Services.DisplayMode;

/// <summary>
/// Сервис для управления режимом отображения (полноэкранный режим).
/// </summary>
public interface IDisplayModeSettingService
{
    /// <summary>
    /// Возвращает true, если включён полноэкранный режим.
    /// </summary>
    bool IsFullScreenMode { get; }
    
    /// <summary>
    /// Переключает полноэкранный режим (вкл/выкл).
    /// </summary>
    Task ToggleFullScreenModeAsync();
    
    /// <summary>
    /// Выходит из полноэкранного режима.
    /// </summary>
    Task ExitFullScreenModeAsync();
    
    /// <summary>
    /// Включает полноэкранный режим.
    /// </summary>
    Task EnterFullScreenModeAsync();
    
    /// <summary>
    /// Получает текущее состояние полноэкранного режима.
    /// </summary>
    Task<bool> GetFullScreenModeStatusAsync();
}