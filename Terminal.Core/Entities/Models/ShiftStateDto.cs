using Terminal.Core.Entities.DbEntities.MainDb;

namespace Terminal.Core.Entities.Models;

/// <summary>
/// Состояние смены.
/// </summary>
public class ShiftStateDto
{
    /// <summary>
    /// Номер смены.
    /// </summary>
    public int ShiftKey { get; set; }
    
    /// <summary>
    /// Запись об открытие смены (должна быть всегда).
    /// </summary>
    public Shift OpenRecord { get; set; } = null!;

    /// <summary>
    /// Запись о закрытие смены (появляется только после закрытия).
    /// </summary>
    public Shift? ClosedRecord { get; set; }
}