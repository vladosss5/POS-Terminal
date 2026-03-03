using System.Windows.Input;

namespace Terminal.ViewModels.Items;

/// <summary>
/// Элемент главного меню (кнопка действия).
/// </summary>
public class MainMenuItemModel
{
    /// <summary>
    /// Название кнопки.
    /// </summary>
    public string? Title { get; set; }
    
    /// <summary>
    /// Команда вызываемая кнопкой.
    /// </summary>
    public ICommand? Command { get; set; }
    
    /// <summary>
    /// Параметр для вызова команда с параметром.
    /// </summary>
    public object? Parameter { get; set; }
}