using Terminal.Core.Enums;

namespace Terminal.Core.Models;

/// <summary>
/// Таблица для отправки.
/// </summary>
public record struct TableToSendDto
{
    /// <summary>
    /// Название таблицы.
    /// </summary>
    public EncashmentTable Name { get; set; }
    
    /// <summary>
    /// Поле являющееся идентификатором.
    /// </summary>
    public string KeyField { get; set; }
    
    /// <summary>
    /// Отображаемое название.
    /// </summary>
    public string DisplayName { get; set; }
    
    /// <summary>
    /// Не отображать если пустое?
    /// </summary>
    public bool DoNotPrintIfEmpty { get; set; }
    
    /// <summary>
    /// Название БД.
    /// </summary>
    public string DbName { get; set; }
}