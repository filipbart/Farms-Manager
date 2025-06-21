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

    public static FarmEntity CreateNew(string name, string nip, string address)
    {
        return new FarmEntity
        {
            Name = name,
            Nip = nip,
            Address = address
        };
    }

    //HenHouses - kurniki
    //Cycles - cykle
}