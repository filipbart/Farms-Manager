using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.FeedAggregate.Entites;

public class FeedInvoiceCorrectionEntity : Entity
{
    protected FeedInvoiceCorrectionEntity()
    {
    }

    public Guid FarmId { get; init; }
    public string InvoiceNumber { get; init; }
    public decimal SubTotal { get; init; }
    public decimal VatAmount { get; init; }
    public decimal InvoiceTotal { get; init; }
    public string FilePath { get; init; }
    public virtual FarmEntity Farm { get; init; }

    public static FeedInvoiceCorrectionEntity CreateNew(
        Guid farmId,
        string invoiceNumber,
        decimal subTotal,
        decimal vatAmount,
        decimal invoiceTotal,
        string filePath,
        Guid? userId = null)
    {
        return new FeedInvoiceCorrectionEntity
        {
            FarmId = farmId,
            InvoiceNumber = invoiceNumber,
            SubTotal = subTotal,
            VatAmount = vatAmount,
            InvoiceTotal = invoiceTotal,
            FilePath = filePath,
            CreatedBy = userId
        };
    }
}