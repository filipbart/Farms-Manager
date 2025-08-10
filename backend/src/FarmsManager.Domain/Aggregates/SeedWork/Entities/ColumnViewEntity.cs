using FarmsManager.Domain.Aggregates.SeedWork.Enums;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.SeedWork.Entities;

public class ColumnViewEntity : Entity
{
    protected ColumnViewEntity()
    {
    }

    public string Name { get; init; }
    public ColumnViewType Type { get; init; }
    public string State { get; init; }

    public static ColumnViewEntity CreateNew(string name, ColumnViewType type, string state, Guid? userId = null)
    {
        return new ColumnViewEntity
        {
            Name = name,
            Type = type,
            State = state,
            CreatedBy = userId
        };
    }
}