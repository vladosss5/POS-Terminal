namespace Terminal.Core.Entities.DbEntities.MainDb;

public class User
{
    /// <summary>
    /// Уникальный ID пользователя (первичный ключ, не автоинкремент)
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Ключ терминала
    /// </summary>
    public decimal? TerminalKey { get; set; }

    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Номер карты
    /// </summary>
    public int? CardNumber { get; set; }

    /// <summary>
    /// Тип пользователя
    /// </summary>
    public int? UserType { get; set; }

    /// <summary>
    /// Пароль пользователя
    /// </summary>
    public string? UserPassword { get; set; }

    /// <summary>
    /// ID эмитента
    /// </summary>
    public int? IssuerId { get; set; }

    /// <summary>
    /// ID организации
    /// </summary>
    public int? OrganisationId { get; set; }

    /// <summary>
    /// Электронный номер карты
    /// </summary>
    public decimal? EcardNumber { get; set; }

    /// <summary>
    /// Связанная организация
    /// </summary>
    public virtual ListOrg Organisation { get; set; } = null!;
}
