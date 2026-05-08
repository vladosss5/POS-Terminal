namespace Terminal.Core.Models;

public class SendTableResult
{
    public bool Success { get; set; }
    public SendTableResponseData? ResponseData { get; set; }
    public string? ErrorMessage { get; set; }
}