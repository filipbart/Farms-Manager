using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.FallenStockAggregate.Entities;

public class UtilizationPlantEntity : Entity
{
    /// <summary>
    /// Nazwa zakładu.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Numer Identyfikacji i Rejestracji Zwierząt (IRZ).
    /// </summary>
    public string IrzNumber { get; private set; }

    /// <summary>
    /// Numer Identyfikacji Podatkowej (NIP).
    /// </summary>
    public string Nip { get; private set; }

    /// <summary>
    /// Adres zakładu.
    /// </summary>
    public string Address { get; private set; }

    protected UtilizationPlantEntity()
    {
    }

    public static UtilizationPlantEntity CreateNew(
        string name,
        string irzNumber,
        string nip,
        string address,
        Guid? userId = null)
    {
        return new UtilizationPlantEntity
        {
            Name = name,
            IrzNumber = irzNumber,
            Nip = nip,
            Address = address,
            CreatedBy = userId
        };
    }

    public void Update(
        string name,
        string irzNumber,
        string nip,
        string address)
    {
        Name = name;
        IrzNumber = irzNumber;
        Nip = nip;
        Address = address;
    }
}