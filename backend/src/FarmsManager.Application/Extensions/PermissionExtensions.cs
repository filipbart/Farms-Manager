using System.ComponentModel;
using System.Reflection;
using FarmsManager.Application.Permissions;

namespace FarmsManager.Application.Extensions;

public static class PermissionExtensions
{
    // Metoda zbierze wszystkie stałe stringi z klasy AppPermissions i ich opisy
    public static Dictionary<string, string> GetAllPermissions()
    {
        var permissions = new Dictionary<string, string>();
        var nestedTypes = typeof(AppPermissions).GetNestedTypes(BindingFlags.Public | BindingFlags.Static);

        foreach (var type in nestedTypes.Concat([typeof(AppPermissions)]))
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            foreach (var field in fields)
            {
                if (field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(string))
                {
                    var value = field.GetValue(null) as string;
                    var description = field.GetCustomAttribute<DescriptionAttribute>()?.Description ?? value;

                    if (!string.IsNullOrEmpty(value))
                    {
                        permissions.TryAdd(value, description);
                    }
                }
            }
        }

        return permissions;
    }
}