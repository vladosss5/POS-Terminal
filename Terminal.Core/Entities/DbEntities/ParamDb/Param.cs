namespace Terminal.Core.Entities.DbEntities.ParamDb;

/// <summary>
/// Параметры приложения.
/// </summary>
public class Param
{
    /// <summary>
    /// Название параметра.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Значение параметра.
    /// </summary>
    public string Value { get; set; } = string.Empty;
}