using System.Windows.Input;

namespace Terminal.ViewModels.Items;

/// <summary>
/// Элемент выбора пункта настройки.
/// </summary>
public record struct SettingsMenuItemModel
{
    /// <summary>
    /// Название кнопки.
    /// </summary>
    public string? Title { get; set; }
    
    /// <summary>
    /// Команда вызываемая кнопкой.
    /// </summary>
    public ICommand? Command { get; set; }
}