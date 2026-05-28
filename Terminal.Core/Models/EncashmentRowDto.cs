namespace Terminal.Core.Models;

public class EncashmentRowDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string TableName { get; set; } = null!;

    public string JsonData { get; set; } = null!;
}