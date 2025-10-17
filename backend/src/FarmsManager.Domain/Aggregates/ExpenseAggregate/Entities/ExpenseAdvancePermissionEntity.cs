using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Enums;
using FarmsManager.Domain.Aggregates.UserAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;

public class ExpenseAdvancePermissionEntity : Entity
{
    public Guid UserId { get; protected internal set; }
    public Guid EmployeeId { get; protected internal set; }
    public ExpenseAdvancePermissionType PermissionType { get; protected internal set; }

    public virtual UserEntity User { get; protected internal set; }
    public virtual EmployeeEntity Employee { get; protected internal set; }

    protected ExpenseAdvancePermissionEntity()
    {
    }

    public static ExpenseAdvancePermissionEntity CreateNew(
        Guid userId,
        Guid employeeId,
        ExpenseAdvancePermissionType permissionType,
        Guid? creatorId = null)
    {
        return new ExpenseAdvancePermissionEntity
        {
            UserId = userId,
            EmployeeId = employeeId,
            PermissionType = permissionType,
            CreatedBy = creatorId
        };
    }

    public void UpdatePermissionType(ExpenseAdvancePermissionType permissionType)
    {
        PermissionType = permissionType;
    }
}
