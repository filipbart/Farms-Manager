using FarmsManager.Domain.Aggregates.FarmAggregate.Enums;
using FarmsManager.Domain.Aggregates.FarmAggregate.Models;

namespace FarmsManager.Domain.Aggregates.FarmAggregate.Entities;

public class SaleEntity
{
    protected SaleEntity()
    {
        // IsSentToIrz = false;
        // DateIrzSentUtc = null;
        OtherExtras = [];
    }

    public static SaleEntity CreateNew(
        Guid farmId,
        Guid cycleId,
        DateOnly saleDate,
        Guid henhouseId,
        SaleType type,
        decimal breederWeight,
        decimal adoptedWeight,
        int adoptedQuantity,
        decimal confiscatedWeight,
        int confiscatedQuantity,
        decimal deadWeight,
        int deadQuantity,
        decimal basePrice,
        decimal priceWithExtras,
        string comment,
        List<SaleOtherExtras> otherExtras)
    {
        return new SaleEntity
        {
            FarmId = farmId,
            CycleId = cycleId,
            SaleDate = saleDate,
            HenhouseId = henhouseId,
            Type = type,
            BreederWeight = breederWeight,
            AdoptedWeight = adoptedWeight,
            AdoptedQuantity = adoptedQuantity,
            ConfiscatedWeight = confiscatedWeight,
            ConfiscatedQuantity = confiscatedQuantity,
            DeadWeight = deadWeight,
            DeadQuantity = deadQuantity,
            BasePrice = basePrice,
            PriceWithExtras = priceWithExtras,
            Comment = comment,
            OtherExtras = otherExtras
        };
    }

    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public DateOnly SaleDate { get; protected internal set; }
    public Guid HenhouseId { get; init; }
    public SaleType Type { get; init; }
    public decimal BreederWeight { get; protected internal set; }

    //Przyjęto:
    public decimal AdoptedWeight { get; protected internal set; }
    public int AdoptedQuantity { get; protected internal set; }

    //Konfiskata
    public decimal ConfiscatedWeight { get; protected internal set; }
    public int ConfiscatedQuantity { get; protected internal set; }

    //Martwe
    public decimal DeadWeight { get; protected internal set; }
    public int DeadQuantity { get; protected internal set; }

    public decimal BasePrice { get; protected internal set; }
    public decimal PriceWithExtras { get; protected internal set; }
    public string Comment { get; protected internal set; }

    public List<SaleOtherExtras> OtherExtras { get; protected internal set; }


    //public Guid InternalGroupId { get; init; }
    //public DateTime? DateIrzSentUtc { get; private set; }
    //public bool IsSentToIrz { get; private set; }
    //public Guid? SentToIrzBy { get; protected internal set; }
    //public string DocumentNumber { get; protected internal set; }
}