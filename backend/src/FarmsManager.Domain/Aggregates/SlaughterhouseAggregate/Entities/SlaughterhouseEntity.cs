using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.SlaughterhouseAggregate.Entities;

public class SlaughterhouseEntity : Entity
{
    protected SlaughterhouseEntity()
    {
    }

    public string Name { get; protected internal set; }

    public string Nip { get; protected internal set; }
    public string Address { get; protected internal set; }
    public string ProducerNumber { get; protected internal set; }

    public static SlaughterhouseEntity CreateNew(string name, string producerNumber, string nip, string address,
        Guid? userId = null)
    {
        return new SlaughterhouseEntity
        {
            Name = name,
            ProducerNumber = producerNumber.Replace(" ", "").Trim(),
            Nip = nip.Replace("PL", "").Replace("-", "").Replace(" ", "").Trim(),
            Address = address,
            CreatedBy = userId
        };
    }
}