using Terminal.Core.Enums;

namespace Terminal.Core.Models;

/// <summary>
/// Модель кнопки на странице входа.
/// </summary>
public record struct LoginButton
{
    /// <summary>
    /// Содержимое кнопки.
    /// </summary>
    public string Content { get; set; }
    
    /// <summary>
    /// Является ли содержимое картинкой.
    /// </summary>
    public bool ContentIsImage { get; set; }

    /// <inheritdoc cref="LoginButtonTypes" />
    public LoginButtonTypes Type { get; set; }
}