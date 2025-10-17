using FarmsManager.Domain.Aggregates.ExpenseAggregate.Enums;

namespace FarmsManager.Application.Models.ExpenseAdvancePermissions;

public class ExpenseAdvancePermissionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ExpenseAdvanceId { get; set; }  // To jest EmployeeId
    public string EmployeeName { get; set; }
    public ExpenseAdvancePermissionType PermissionType { get; set; }
    public DateTime DateCreatedUtc { get; set; }
    public DateTime? DateModifiedUtc { get; set; }
}
