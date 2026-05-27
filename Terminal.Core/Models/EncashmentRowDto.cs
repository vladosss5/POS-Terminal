namespace Terminal.Core.Models;

public class EncashmentRowDto
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string JsonData { get; set; } = null!;
}