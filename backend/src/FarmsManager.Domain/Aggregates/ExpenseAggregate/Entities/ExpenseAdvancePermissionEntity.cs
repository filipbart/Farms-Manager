using FarmsManager.Domain.Aggregates.ExpenseAggregate.Enums;
using FarmsManager.Domain.Aggregates.UserAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;

public class ExpenseAdvancePermissionEntity : Entity
{
    public Guid UserId { get; protected internal set; }
    public Guid ExpenseAdvanceRegistryId { get; protected internal set; }
    public ExpenseAdvancePermissionType PermissionType { get; protected internal set; }

    public virtual UserEntity User { get; protected internal set; }
    public virtual ExpenseAdvanceRegistryEntity ExpenseAdvanceRegistry { get; protected internal set; }

    protected ExpenseAdvancePermissionEntity()
    {
    }

    public static ExpenseAdvancePermissionEntity CreateNew(
        Guid userId,
        Guid expenseAdvanceRegistryId,
        ExpenseAdvancePermissionType permissionType,
        Guid? creatorId = null)
    {
        return new ExpenseAdvancePermissionEntity
        {
            UserId = userId,
            ExpenseAdvanceRegistryId = expenseAdvanceRegistryId,
            PermissionType = permissionType,
            CreatedBy = creatorId
        };
    }

    public void UpdatePermissionType(ExpenseAdvancePermissionType permissionType)
    {
        PermissionType = permissionType;
    }
}
