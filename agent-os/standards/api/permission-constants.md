---
# Permission Constants Structure

Define permissions as static constants in AppPermissions:

```csharp
public static class AppPermissions
{
    [PermissionGroup("Dane")]
    public static class Data
    {
        [Description("Zarządzanie fermami")]
        public const string FarmsManage = "data:farms:manage";
    }
}
```

- Naming convention: `area:subarea:action` (e.g., `data:farms:manage`)
- PermissionGroup and Description attributes are used by PermissionExtensions for UI generation
- Use nested static classes for logical grouping
- All permissions are discoverable via reflection for admin UI

```csharp
// Usage in controllers
[HasPermission(AppPermissions.Data.FarmsManage)]
```
