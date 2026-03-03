using System.Windows.Input;

namespace Terminal.ViewModels.Items;

/// <summary>
/// Элемент главного меню (кнопка действия).
/// </summary>
public record struct MainMenuItemModel
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