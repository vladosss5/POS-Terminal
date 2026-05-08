namespace Terminal.Core.Models;

public class SendTableResponseData
{
    public List<long> SuccessKeys { get; set; } = [];
    public List<long> ErrorKeys { get; set; } = [];
    public List<long> ErrorSaveKeys { get; set; } = [];
}