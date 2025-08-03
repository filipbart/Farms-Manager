using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.SaleAggregate.Enums;
using FarmsManager.Domain.Aggregates.SaleAggregate.Models;
using FarmsManager.Domain.Aggregates.SlaughterhouseAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.SaleAggregate.Entities;

public class SaleEntity : Entity
{
    protected SaleEntity()
    {
        IsSentToIrz = false;
        DateIrzSentUtc = null;
        OtherExtras = [];
    }

    public static SaleEntity CreateNew(
        Guid internalGroupId,
        SaleType type,
        DateOnly saleDate,
        Guid farmId,
        Guid cycleId,
        Guid slaughterhouseId,
        Guid henhouseId,
        decimal weight,
        int quantity,
        decimal confiscatedWeight,
        int confiscatedCount,
        decimal deadWeight,
        int deadCount,
        decimal farmerWeight,
        decimal basePrice,
        decimal priceWithExtras,
        string comment,
        IEnumerable<SaleOtherExtras> otherExtras,
        Guid? userId = null)
    {
        return new SaleEntity
        {
            InternalGroupId = internalGroupId,
            Type = type,
            SaleDate = saleDate,
            FarmId = farmId,
            CycleId = cycleId,
            SlaughterhouseId = slaughterhouseId,
            HenhouseId = henhouseId,
            Weight = weight,
            Quantity = quantity,
            ConfiscatedWeight = confiscatedWeight,
            ConfiscatedCount = confiscatedCount,
            DeadWeight = deadWeight,
            DeadCount = deadCount,
            FarmerWeight = farmerWeight,
            BasePrice = basePrice,
            PriceWithExtras = priceWithExtras,
            Comment = comment,
            OtherExtras = otherExtras?.ToList(),
            CreatedBy = userId
        };
    }

    public SaleType Type { get; init; }
    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public DateOnly SaleDate { get; protected internal set; }
    public Guid SlaughterhouseId { get; init; }
    public Guid HenhouseId { get; init; }

    //Przyjęto:
    public decimal Weight { get; protected internal set; }
    public int Quantity { get; protected internal set; }

    //Konfiskata
    public decimal ConfiscatedWeight { get; protected internal set; }
    public int ConfiscatedCount { get; protected internal set; }

    //Martwe
    public decimal DeadWeight { get; protected internal set; }
    public int DeadCount { get; protected internal set; }

    public decimal FarmerWeight { get; protected internal set; }
    public decimal BasePrice { get; protected internal set; }
    public decimal PriceWithExtras { get; protected internal set; }
    public string Comment { get; protected internal set; }

    public List<SaleOtherExtras> OtherExtras { get; protected internal set; }

    public virtual FarmEntity Farm { get; init; }
    public virtual CycleEntity Cycle { get; init; }
    public virtual SlaughterhouseEntity Slaughterhouse { get; init; }
    public virtual HenhouseEntity Henhouse { get; init; }

    //Dane IRZplus
    public Guid InternalGroupId { get; init; }
    public DateTime? DateIrzSentUtc { get; private set; }
    public bool IsSentToIrz { get; private set; }
    public Guid? SentToIrzBy { get; protected internal set; }
    public string DocumentNumber { get; protected internal set; }
    public string DirectoryPath { get; protected internal set; }

    public void Update(
        DateOnly saleDate,
        decimal weight,
        int quantity,
        decimal confiscatedWeight,
        int confiscatedCount,
        decimal deadWeight,
        int deadCount,
        decimal farmerWeight,
        decimal basePrice,
        decimal priceWithExtras,
        string comment,
        IEnumerable<SaleOtherExtras> otherExtras)
    {
        SaleDate = saleDate;
        Weight = weight;
        Quantity = quantity;
        ConfiscatedWeight = confiscatedWeight;
        ConfiscatedCount = confiscatedCount;
        DeadWeight = deadWeight;
        DeadCount = deadCount;
        FarmerWeight = farmerWeight;
        BasePrice = basePrice;
        PriceWithExtras = priceWithExtras;
        Comment = comment;

        OtherExtras.Clear();
        if (otherExtras != null)
        {
            OtherExtras.AddRange(otherExtras);
        }
    }

    public void MarkAsSentToIrz(string documentNumber, Guid userId)
    {
        IsSentToIrz = true;
        SentToIrzBy = userId;
        DateIrzSentUtc = DateTime.UtcNow;
        DocumentNumber = documentNumber;
    }

    public void MarkAsSentToIrzByAdmin(Guid userId)
    {
        IsSentToIrz = true;
        SentToIrzBy = userId;
    }

    public bool IsAlreadySentToIrz() => DateIrzSentUtc.HasValue || IsSentToIrz;
    public void SetDirectoryPath(string directoryPath) => DirectoryPath = directoryPath;
}