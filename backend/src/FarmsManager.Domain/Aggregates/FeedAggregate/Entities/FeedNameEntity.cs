using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.FeedAggregate.Entities;

public class FeedNameEntity : Entity
{
    protected FeedNameEntity()
    {
    }

    public string Name { get; init; }

    public static FeedNameEntity CreateNew(string name, Guid? userId = null)
    {
        return new FeedNameEntity
        {
            Name = name,
            CreatedBy = userId
        };
    }
}