namespace Terminal.Core.DbEntities.MainDb;

public class Allow
{
    /// <summary>
    /// Первичный ключ записи разрешения
    /// </summary>
    public int AllowKey { get; set; }

    /// <summary>
    /// Ключ магазина / точки продажи
    /// </summary>
    public int? ShopKey { get; set; }

    /// <summary>
    /// Ключ эмитента карты / системы
    /// </summary>
    public int? IssuerKey { get; set; }

    /// <summary>
    /// Сервер, с которого пришёл запрос
    /// </summary>
    public string? RequestServer { get; set; }
}
