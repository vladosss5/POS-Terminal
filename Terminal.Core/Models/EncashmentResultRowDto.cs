using Terminal.Core.Enums;

namespace Terminal.Core.Models;

/// <summary>
/// Модель строки результата инкассации.
/// </summary>
public class EncashmentResultRowDto
{
    /// <summary>
    /// Идентификатор строки.
    /// </summary>
    public string IdEntityFromTable { get; set; } = null!;
    
    /// <summary>
    /// Название таблицы откуда строка.
    /// </summary>
    public EncashmentTable TableName { get; set; }
    
    /// <summary>
    /// Успешность вставки в БД TMS.
    /// </summary>
    public bool Success { get; set; }
}