using Terminal.Core.Enums;

namespace Terminal.Core.Models;

public class EncashmentRowDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public EncashmentTable TableName { get; set; }

    public string JsonData { get; set; } = null!;
}