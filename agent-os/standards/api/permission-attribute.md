---
# Permission-Based Authorization

Use HasPermission attribute for endpoint-level permissions:

```csharp
[HasPermission(AppPermissions.Data.FarmsManage)]
public async Task<IActionResult> AddFarm(AddFarmCommand command)
```

- Checked by CheckPermissionMiddleware before controller execution
- Throws DomainException.Forbidden() if permission check fails
- Endpoints without HasPermission are accessible to any authenticated user
- Use [AllowAnonymous] for public endpoints (only AuthController)

```csharp
// Permission hierarchy in AppPermissions
AppPermissions.Data.FarmsManage = "data:farms:manage"
AppPermissions.Settings.Users.Manage = "settings:users:manage"
```
