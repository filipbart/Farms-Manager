using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;

public class HatcheryPriceEntity : Entity
{
    public Guid HatcheryId { get; init; }
    public decimal Price { get; protected internal set; }
    public DateOnly Date { get; protected internal set; }
    public string Comment { get; protected internal set; }

    public virtual HatcheryEntity Hatchery { get; set; }

    public static HatcheryPriceEntity CreateNew(
        Guid hatcheryId,
        decimal price,
        DateOnly date,
        string comment,
        Guid? userId = null)
    {
        return new HatcheryPriceEntity
        {
            HatcheryId = hatcheryId,
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