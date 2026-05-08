namespace Terminal.Core.Models;

public class EncashmentResult
{
    public bool Success { get; set; }
    public bool HasData { get; set; }
    public bool NeedRestart { get; set; }
    public IncassationData Data { get; set; } = new();
    public string? ErrorMessage { get; set; }
}