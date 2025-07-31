using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.GasAggregate.Entities;

public class GasContractorEntity : Entity
{
    public string Name { get; protected internal set; }
    public string Nip { get; protected internal set; }
    public string Address { get; protected internal set; }

    public static GasContractorEntity CreateNew(string name, string nip, string address,
        Guid? userId = null)
    {
        return new GasContractorEntity
        {
            Name = name,
            Nip = nip.Replace("-", "").Trim(),
            Address = address,
            CreatedBy = userId
        };
    }
}