namespace FarmsManager.Application.Models.ExpenseAdvancePermissions;

public class UserExpenseAdvancePermissionsDto
{
    public Guid UserId { get; set; }
    public List<ExpenseAdvancePermissionDto> Permissions { get; set; }
}
