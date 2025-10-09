using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.FeedAggregate.Entities;

public class FeedInvoiceCorrectionEntity : Entity
{
    protected FeedInvoiceCorrectionEntity()
    {
    }

    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public string InvoiceNumber { get; protected internal set; }
    public decimal SubTotal { get; protected internal set; }
    public decimal VatAmount { get; protected internal set; }
    public decimal InvoiceTotal { get; protected internal set; }
    public DateOnly InvoiceDate { get; protected internal set; }
    public string FilePath { get; protected internal set; }
    public virtual FarmEntity Farm { get; init; }
    public virtual CycleEntity Cycle { get; init; }

    public DateTime? PaymentDateUtc { get; protected internal set; }
    public Guid? PaymentId { get; protected internal set; }

    public void SetFilePath(string filePath) => FilePath = filePath;

    public void MarkAsPaid(Guid paymentId)
    {
        PaymentId = paymentId;
        PaymentDateUtc = DateTime.UtcNow;
    }

    public void MarkAsUnpaid()
    {
        PaymentId = null;
        PaymentDateUtc = null;
    }

    public static FeedInvoiceCorrectionEntity CreateNew(
        Guid farmId,
        Guid cycleId,
        string invoiceNumber,
        decimal subTotal,
        decimal vatAmount,
        decimal invoiceTotal,
        DateOnly invoiceDate,
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
            CreatedBy = userId
        };
    }

    public void Update(string invoiceNumber, decimal subTotal, decimal vatAmount, decimal invoiceTotal,
        DateOnly invoiceDate)
    {
        InvoiceNumber = invoiceNumber;
        SubTotal = subTotal;
        VatAmount = vatAmount;
        InvoiceTotal = invoiceTotal;
        InvoiceDate = invoiceDate;
    }
}