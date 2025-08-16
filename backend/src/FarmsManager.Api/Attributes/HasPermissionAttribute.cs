namespace FarmsManager.Api.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class HasPermissionAttribute : Attribute
{
    public HasPermissionAttribute(string permission)
    {
        Permission = permission;
    }

    public string Permission { get; }
}