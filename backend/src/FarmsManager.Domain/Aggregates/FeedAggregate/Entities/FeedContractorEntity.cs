using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.FeedAggregate.Entities;

public class FeedContractorEntity : Entity
{
    protected FeedContractorEntity()
    {
    }

    public string Name { get; protected internal set; }
    public string Nip { get; protected internal set; }

    public static FeedContractorEntity CreateNewFromInvoice(string name, string nip, Guid? userId = null)
    {
        return new FeedContractorEntity
        {
            Name = name,
            Nip = nip.Replace("PL", "").Replace("-", "").Replace(" ", "").Trim(),
            CreatedBy = userId
        };
    }
}