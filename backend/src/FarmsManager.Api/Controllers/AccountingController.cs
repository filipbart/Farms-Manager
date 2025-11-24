using FarmsManager.Api.Attributes;
using FarmsManager.Application.Permissions;

namespace FarmsManager.Api.Controllers;

[HasPermission(AppPermissions.Accounting.Manage)]
public class AccountingController
{
    
}