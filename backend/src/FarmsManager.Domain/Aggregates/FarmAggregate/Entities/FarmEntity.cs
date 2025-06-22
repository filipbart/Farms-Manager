using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.FarmAggregate.Entities;

public class FarmEntity : Entity
{
    protected FarmEntity()
    {
    }

    public string Name { get; protected internal set; }
    public string Nip { get; protected internal set; }
    public string Address { get; protected internal set; }

    /// <summary>
    /// Kurniki
    /// </summary>
    private readonly List<HenhouseEntity> _henhouses = [];

    public virtual IReadOnlyCollection<HenhouseEntity> Henhouses => _henhouses.AsReadOnly();

    public static FarmEntity CreateNew(string name, string nip, string address, Guid? createdBy = null)
    {
        return new FarmEntity
        {
            Name = name,
            Nip = nip,
            Address = address,
            CreatedBy = createdBy
        };
    }

    //Cycles - cykle
}