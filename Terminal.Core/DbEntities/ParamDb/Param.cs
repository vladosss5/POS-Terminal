namespace Terminal.Core.DbEntities.ParamDb;

/// <summary>
/// Парамерты приложения.
/// </summary>
public class Param
{
    /// <summary>
    /// Название парамерта.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Значение параметра.
    /// </summary>
    public string Value { get; set; } = string.Empty;
}