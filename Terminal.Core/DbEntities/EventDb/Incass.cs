namespace Terminal.Core.DbEntities.EventDb;

/// <summary>
/// Модель таблицы инкассаций из EventDb.
/// </summary>
public partial class Incass
{
    /// <summary>
    /// Номер инкассации.
    /// </summary>
    public int IncassKey { get; set; }

    /// <summary>
    /// Дата начала последней инкассации.
    /// </summary>
    public DateTime? LastDatetimeStart { get; set; }

    /// <summary>
    /// Дата окончания последней инкассации.
    /// </summary>
    public DateTime? LastDatetimeEnd { get; set; }

    /// <summary>
    /// Какие-то флаги TODO: какие?
    /// </summary>
    public int? Flags { get; set; }
}