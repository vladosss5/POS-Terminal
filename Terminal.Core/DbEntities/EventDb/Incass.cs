namespace Terminal.Core.DbEntities.EventDb;

public partial class Incass
{
    public int IncassKey { get; set; }

    public DateTime? LastDatetimeStart { get; set; }

    public DateTime? LastDatetimeEnd { get; set; }

    public int? Flags { get; set; }
}