using Terminal.Core.Enums;

namespace Terminal.Core.Models;

/// <summary>
/// Строка с данными инкассации.
/// </summary>
public class EncashmentRowDto
{
    /// <summary>
    /// Идентификатор строки.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Название таблицы откуда данные.
    /// </summary>
    public EncashmentTable TableName { get; set; }

    /// <summary>
    /// Данные (строка из таблицы БД).
    /// </summary>
    public string JsonData { get; set; } = null!;
}