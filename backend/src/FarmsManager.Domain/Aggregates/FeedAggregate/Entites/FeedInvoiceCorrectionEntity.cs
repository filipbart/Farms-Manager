using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.FeedAggregate.Entites;

public class FeedInvoiceCorrectionEntity : Entity
{
    protected FeedInvoiceCorrectionEntity()
    {
    }

    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public string InvoiceNumber { get; init; }
    public decimal SubTotal { get; init; }
    public decimal VatAmount { get; init; }
    public decimal InvoiceTotal { get; init; }
    public DateOnly InvoiceDate { get; init; }
    public string FilePath { get; init; }
    public virtual FarmEntity Farm { get; init; }
    public virtual CycleEntity Cycle { get; init; }

    public static FeedInvoiceCorrectionEntity CreateNew(
        Guid farmId,
        Guid cycleId,
        string invoiceNumber,
        decimal subTotal,
        decimal vatAmount,
        decimal invoiceTotal,
        DateOnly invoiceDate,
        string filePath,
        Guid? userId = null)
    {
        return new FeedInvoiceCorrectionEntity
        {
            FarmId = farmId,
            CycleId = cycleId,
            InvoiceNumber = invoiceNumber,
            SubTotal = subTotal,
            VatAmount = vatAmount,
            InvoiceTotal = invoiceTotal,
            InvoiceDate = invoiceDate,
            FilePath = filePath,
            CreatedBy = userId
        };
    }
}