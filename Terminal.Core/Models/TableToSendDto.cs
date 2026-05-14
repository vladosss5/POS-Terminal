namespace Terminal.Core.Models;

public record struct TableToSendDto
{
    public string Name { get; set; }
    
    public string KeyField { get; set; }
    
    public string DisplayName { get; set; }
    
    public bool DoNotPrintIfEmpty { get; set; }
    
    public string DbName { get; set; }
}