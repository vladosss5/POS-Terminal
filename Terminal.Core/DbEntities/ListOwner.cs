using System;
using System.Collections.Generic;

namespace Terminal.Core.DbEntities;

public partial class ListOwner
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
