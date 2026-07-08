using Avalonia.Media.Imaging;
using Terminal.Core.Enums;

namespace Terminal.Core.Models;

/// <summary>
/// Статус приложения.
/// </summary>
public class Status
{
    /// <summary>
    /// Тип статуса.
    /// </summary>
    public StatusType? Type { get; set; }
    
    /// <summary>
    /// Иконка для отображения.
    /// </summary>
    public Bitmap? Icon { get; set; }
}