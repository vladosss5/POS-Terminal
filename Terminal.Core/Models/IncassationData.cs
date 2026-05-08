namespace Terminal.Core.Models;

/// <summary>
/// Данные для инкассации
/// </summary>
public class IncassationData
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool AuthSuccess { get; set; }
    public List<IncassationItem> Items { get; set; } = new();
}