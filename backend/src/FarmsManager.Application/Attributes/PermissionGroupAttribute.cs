namespace FarmsManager.Application.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class PermissionGroupAttribute : Attribute
{
    public string Name { get; }

    public PermissionGroupAttribute(string name)
    {
        Name = name;
    }
}