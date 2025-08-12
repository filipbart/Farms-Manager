using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.FarmAggregate.Entities;

public class HenhouseEntity : Entity
{
    protected HenhouseEntity()
    {
    }

    public Guid FarmId { get; init; }
    public string Name { get; protected internal set; }
    public string Code { get; protected internal set; }
    public int Area { get; protected internal set; }
    public string Description { get; protected internal set; }
    public virtual FarmEntity Farm { get; init; }

    public static HenhouseEntity CreateNew(string name, string code, int area, string description, Guid farmId,
        Guid? userId = null)
    {
        return new HenhouseEntity
        {
            Name = name,
            Code = code,
            Area = area,
            Description = description,
            FarmId = farmId,
            CreatedBy = userId
        };
    }

    public void Update(string name, string code, int area, string description)
    {
        Name = name;
        Code = code;
        Area = area;
        Description = description;
    }
}