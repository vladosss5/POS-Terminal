namespace Terminal.Core.Models;

public class SendTableResult
{
    public bool Success { get; set; }
    public List<long> SuccessKeys { get; set; } = [];
    public List<long> ErrorKeys { get; set; } = [];
    public List<long> ErrorSaveKeys { get; set; } = [];
}