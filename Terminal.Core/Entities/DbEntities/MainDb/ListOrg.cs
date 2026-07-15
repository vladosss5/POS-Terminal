namespace Terminal.Core.Entities.DbEntities.MainDb;

public class ListOrg
{
    /// <summary>
    /// Первичный ключ записи организации
    /// </summary>
    public int ListOrgKey { get; set; }

    /// <summary>
    /// Ключ организации (используется как AlternateKey)
    /// </summary>
    public int? OrganisationKey { get; set; }

    /// <summary>
    /// Наименование организации
    /// </summary>
    public string? OrganisationName { get; set; }
}
