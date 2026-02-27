using System;
using System.Collections.Generic;

namespace Terminal.Core.DbEntities;

public partial class CardPassword
{
    /// <summary>
    /// Первичный ключ пароля карты
    /// </summary>
    public int CardPasswordKey { get; set; }

    /// <summary>
    /// Графический номер карты
    /// </summary>
    public long? GraphicalNumber { get; set; }

    /// <summary>
    /// Электронный номер карты
    /// </summary>
    public long? ElectronicNumber { get; set; }

    /// <summary>
    /// Графический номер UPOS
    /// </summary>
    public long? GraphicalNumberUpos { get; set; }

    /// <summary>
    /// Пароль карты
    /// </summary>
    public int? Password { get; set; }

    /// <summary>
    /// Начало последней сессии (timestamp)
    /// </summary>
    public long? LastSessionStart { get; set; }

    /// <summary>
    /// Окончание последней сессии (timestamp)
    /// </summary>
    public long? LastSessionEnd { get; set; }

    /// <summary>
    /// Тип продажи
    /// </summary>
    public int? SaleType { get; set; }
}
