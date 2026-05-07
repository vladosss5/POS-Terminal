namespace Terminal.Core.Models;

public class IncassationResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; }
    public IncassationData? Data { get; set; }
}