namespace Terminal.Core.Models;

/// <summary>
/// Модель строки результата инкассации.
/// </summary>
public class EncashmentResultRowDto
{
    /// <summary>
    /// Идентификатор строки.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Название таблицы откуда строка.
    /// </summary>
    public string TableName { get; set; } = null!;

    /// <summary>
    /// Идентификатор записи внутри таблицы.
    /// </summary>
    public long IdRowFromTable { get; set; }
    
    /// <summary>
    /// Успешность вставки в БД TMS.
    /// </summary>
    public bool Success { get; set; }
}