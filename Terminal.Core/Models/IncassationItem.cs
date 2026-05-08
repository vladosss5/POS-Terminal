namespace Terminal.Core.Models;

public class IncassationItem
{
    public string TableName { get; set; } = string.Empty;
    public string TableKey { get; set; } = string.Empty;
    public int IncassBefore { get; set; }
    public int IncassAfter { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool DoNotPrintIfEmpty { get; set; }
}