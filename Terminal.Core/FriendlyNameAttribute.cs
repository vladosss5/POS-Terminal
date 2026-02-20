namespace Terminal.Core;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class FriendlyNameAttribute : Attribute
{
    public string Name { get; }
    
    public FriendlyNameAttribute(string name)
    {
        Name = name;
    }
}