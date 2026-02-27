namespace Terminal.Core;

/// <summary>
/// Работа с атрибутикой enum
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class FriendlyNameAttribute : Attribute
{
    /// <summary>
    /// Наименование.
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="name">Наименование.</param>
    public FriendlyNameAttribute(string name)
    {
        Name = name;
    }
}