using FarmsManager.Domain.Aggregates.ExpenseAggregate.Enums;

namespace FarmsManager.Application.Models.ExpenseAdvancePermissions;

public class ExpenseAdvancePermissionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ExpenseAdvanceRegistryId { get; set; }
    public string ExpenseAdvanceRegistryName { get; set; }
    public ExpenseAdvancePermissionType PermissionType { get; set; }
    public DateTime DateCreatedUtc { get; set; }
    public DateTime? DateModifiedUtc { get; set; }
}
