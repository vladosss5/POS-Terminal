namespace Terminal.Core.Entities.DbEntities.MainDb;

public class ListOwner
{
    /// <summary>
    /// Первичный ключ записи владельца
    /// </summary>
    public int ListOwnerKey { get; set; }

    /// <summary>
    /// Ключ владельца (используется как AlternateKey)
    /// </summary>
    public int? OwnerKey { get; set; }

    /// <summary>
    /// Ключ организации
    /// </summary>
    public int? OrganisationKey { get; set; }

    /// <summary>
    /// Наименование владельца
    /// </summary>
    public string? OwnerName { get; set; }

    /// <summary>
    /// Графический номер
    /// </summary>
    public string? GraphicalNumber { get; set; }
}
