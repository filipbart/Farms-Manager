using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.GasAggregate.Entities;

public class GasDeliveryEntity : Entity
{
    public Guid FarmId { get; init; }
    public Guid GasContractorId { get; init; }
    public string InvoiceNumber { get; protected internal set; }
    public DateOnly InvoiceDate { get; protected internal set; }
    public decimal InvoiceTotal { get; protected internal set; }
    public decimal UnitPrice { get; protected internal set; }
    public decimal Quantity { get; protected internal set; }
    public string Comment { get; protected internal set; }
    public string FilePath { get; protected internal set; }

    public decimal UsedQuantity { get; private set; }

    public virtual FarmEntity Farm { get; set; }
    public virtual GasContractorEntity GasContractor { get; set; }

    public void SetFilePath(string filePath) => FilePath = filePath;

    public void AddUsedQuantity(decimal quantity)
    {
        if (quantity > Quantity - UsedQuantity)
        {
            throw new Exception("Próba zużycia większej ilości gazu niż dostępna w dostawie.");
        }

        UsedQuantity += quantity;
    }

    public static GasDeliveryEntity CreateNew(
        Guid farmId,
        Guid gasContractorId,
        string invoiceNumber,
        DateOnly invoiceDate,
        decimal invoiceTotal,
        decimal unitPrice,
        decimal quantity,
        string comment,
        Guid? userId = null)
    {
        return new GasDeliveryEntity
        {
            FarmId = farmId,
            GasContractorId = gasContractorId,
            InvoiceNumber = invoiceNumber,
            InvoiceDate = invoiceDate,
            InvoiceTotal = invoiceTotal,
            UnitPrice = unitPrice,
            Quantity = quantity,
            Comment = comment,
            CreatedBy = userId
        };
    }

    public void Update(
        string invoiceNumber,
        DateOnly invoiceDate,
        decimal invoiceTotal,
        decimal unitPrice,
        decimal quantity,
        string comment)
    {
        InvoiceNumber = invoiceNumber;
        InvoiceDate = invoiceDate;
        InvoiceTotal = invoiceTotal;
        UnitPrice = unitPrice;
        Quantity = quantity;
        Comment = comment;
    }
}