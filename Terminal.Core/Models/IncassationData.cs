namespace Terminal.Core.Models;

/// <summary>
/// Данные для инкассации
/// </summary>
public class IncassationData
{
    /// <summary>
    /// Номер смены (количество дней от 2010-01-01)
    /// </summary>
    public int ShiftKey { get; set; }
    
    /// <summary>
    /// Дата события инкассации
    /// </summary>
    public DateTime EventDate { get; set; }
    
    /// <summary>
    /// Количество запретов в системе
    /// </summary>
    public int ProhibitionCount { get; set; }
    
    /// <summary>
    /// Количество разрешений в системе
    /// </summary>
    public int AllowCount { get; set; }
    
    /// <summary>
    /// Количество корректировок (обновлений)
    /// </summary>
    public int UpdateCount { get; set; }
    
    /// <summary>
    /// Количество продаж (ASW)
    /// </summary>
    public int AswCount { get; set; }
    
    /// <summary>
    /// Количество возвратов (AS)
    /// </summary>
    public int AsCount { get; set; }
    
    /// <summary>
    /// Количество платежей (APL)
    /// </summary>
    public int AplCount { get; set; }
    
    /// <summary>
    /// Количество корректировок (ACD)
    /// </summary>
    public int AcdCount { get; set; }
    
    /// <summary>
    /// Количество информации о сменах (ABE)
    /// </summary>
    public int AbeCount { get; set; }
    
    /// <summary>
    /// Количество информации о ТРК (AO)
    /// </summary>
    public int AoCount { get; set; }
}