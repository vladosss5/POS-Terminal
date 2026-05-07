namespace Terminal.Core.Models;

public class ReceiveResult
{
    public int PacketCount { get; set; }
    public List<string> SavedFiles { get; set; } = [];
    public bool Success { get; set; }
}