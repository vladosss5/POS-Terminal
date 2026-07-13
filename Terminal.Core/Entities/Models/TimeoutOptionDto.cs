namespace Terminal.Core.Entities.Models;

/// <summary>
/// Варианты времени ожидания.
/// </summary>
public record struct TimeoutOptionDto
{
    /// <summary>
    /// Кол-во секунд.
    /// </summary>
    public short Seconds { get; set; }
    
    /// <summary>
    /// Св-во для показа секунд.
    /// </summary>
    public string DisplayName => $"{Seconds} сек.";
}