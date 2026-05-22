namespace Terminal.Core.DbEntities.MainDb;

public partial class Prohibition
{
    /// <summary>
    /// Первичный ключ запрета
    /// </summary>
    public int ProhibitionKey { get; set; }

    /// <summary>
    /// Ключ магазина
    /// </summary>
    public int? ShopKey { get; set; }

    /// <summary>
    /// Ключ эмитента
    /// </summary>
    public int? IssuerKey { get; set; }

    /// <summary>
    /// Ключ организации
    /// </summary>
    public int? OrganisationKey { get; set; }

    /// <summary>
    /// Ключ владельца
    /// </summary>
    public int? OwnerKey { get; set; }

    /// <summary>
    /// Дата начала действия запрета
    /// </summary>
    public DateTime? BeginDate { get; set; }

    /// <summary>
    /// Признак (знак) запрета
    /// </summary>
    public byte? Sign { get; set; }

    /// <summary>
    /// Дата окончания действия запрета
    /// </summary>
    public DateTime? EndDate { get; set; }
}
