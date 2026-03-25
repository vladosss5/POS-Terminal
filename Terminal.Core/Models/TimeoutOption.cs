namespace Terminal.Core.Models;

/// <summary>
/// Варианты времени ожидания.
/// </summary>
public record struct TimeoutOption
{
    /// <summary>
    /// Кол-во секунд.
    /// </summary>
    public int Seconds { get; set; }
    
    /// <summary>
    /// Св-во для показа секунд.
    /// </summary>
    public string DisplayName => $"{Seconds} сек.";
}