using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;

public class ExpenseAdvanceRegistryEntity : Entity
{
    public string Name { get; protected internal set; }
    public string Description { get; protected internal set; }
    public bool IsActive { get; protected internal set; }

    private readonly List<ExpenseAdvancePermissionEntity> _permissions = new();
    public virtual IReadOnlyCollection<ExpenseAdvancePermissionEntity> Permissions => _permissions;

    protected ExpenseAdvanceRegistryEntity()
    {
    }

    public static ExpenseAdvanceRegistryEntity CreateNew(
        string name,
        string description,
        Guid? userId = null)
    {
        return new ExpenseAdvanceRegistryEntity
        {
            Name = name,
            Description = description,
            IsActive = true,
            CreatedBy = userId
        };
    }

    public void Update(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
