using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;

public class HatcheryPriceEntity : Entity
{
    public decimal Price { get; protected internal set; }
    public DateOnly Date { get; protected internal set; }
    public string Comment { get; protected internal set; }
    public string HatcheryName { get; protected internal set; }

    public static HatcheryPriceEntity CreateNew(
        string hatcheryName,
        decimal price,
        DateOnly date,
        string comment,
        Guid? userId = null)
    {
        return new HatcheryPriceEntity
        {
            HatcheryName = hatcheryName,
            Price = price,
            Date = date,
            Comment = comment,
            CreatedBy = userId
        };
    }

    public void Update(
        decimal price,
        DateOnly date,
        string comment)
    {
        Price = price;
        Date = date;
        Comment = comment;
    }
}