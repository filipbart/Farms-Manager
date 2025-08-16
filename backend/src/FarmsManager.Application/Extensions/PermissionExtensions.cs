using System.ComponentModel;
using System.Reflection;
using FarmsManager.Application.Attributes;
using FarmsManager.Application.Permissions;
using FarmsManager.Application.Queries.Users;

namespace FarmsManager.Application.Extensions;

public static class PermissionExtensions
{
    public static List<PermissionModel> GetAllPermissions(this Type type)
    {
        var permissions = new List<PermissionModel>();
        GetPermissionsRecursively(type, permissions);
        return permissions;
    }

    private static void GetPermissionsRecursively(Type type, List<PermissionModel> permissions)
    {
        var groupName = type.GetCustomAttribute<PermissionGroupAttribute>()?.Name
                        ?? (type.Name == nameof(AppPermissions) ? "Ogólne" : type.Name);


        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string));

        foreach (var field in fields)
        {
            var permissionName = (string)field.GetValue(null);
            var description = field.GetCustomAttribute<DescriptionAttribute>()?.Description ?? permissionName;

            if (!string.IsNullOrEmpty(permissionName))
            {
                permissions.Add(new PermissionModel
                {
                    Name = permissionName,
                    Description = description,
                    Group = groupName
                });
            }
        }

        var nestedTypes = type.GetNestedTypes(BindingFlags.Public | BindingFlags.Static);
        foreach (var nestedType in nestedTypes)
        {
            GetPermissionsRecursively(nestedType, permissions);
        }
    }
}