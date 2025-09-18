using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.SlaughterhouseAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.SaleAggregate.Entities;

public class SaleInvoiceEntity : Entity
{
    public Guid FarmId { get; init; }
    public Guid CycleId { get; protected internal set; }
    public Guid SlaughterhouseId { get; init; }
    public string InvoiceNumber { get; protected internal set; }
    public DateOnly InvoiceDate { get; protected internal set; }
    public DateOnly DueDate { get; protected internal set; }
    public decimal InvoiceTotal { get; protected internal set; }
    public decimal SubTotal { get; protected internal set; }
    public decimal VatAmount { get; protected internal set; }
    public string FilePath { get; protected internal set; }
    public DateOnly? PaymentDate { get; protected internal set; }

    public virtual FarmEntity Farm { get; init; }
    public virtual CycleEntity Cycle { get; init; }
    public virtual SlaughterhouseEntity Slaughterhouse { get; init; }

    protected SaleInvoiceEntity()
    {
    }

    public static SaleInvoiceEntity CreateNew(
        Guid farmId,
        Guid cycleId,
        Guid slaughterhouseId,
        string invoiceNumber,
        DateOnly invoiceDate,
        DateOnly dueDate,
        decimal invoiceTotal,
        decimal subTotal,
        decimal vatAmount,
        Guid? userId = null)
    {
        return new SaleInvoiceEntity
        {
            FarmId = farmId,
            CycleId = cycleId,
            SlaughterhouseId = slaughterhouseId,
            InvoiceNumber = invoiceNumber,
            InvoiceDate = invoiceDate,
            DueDate = dueDate,
            InvoiceTotal = invoiceTotal,
            SubTotal = subTotal,
            VatAmount = vatAmount,
            CreatedBy = userId
        };
    }

    public void Update(
        string invoiceNumber,
        DateOnly invoiceDate,
        DateOnly dueDate,
        DateOnly? paymentDate,
        decimal invoiceTotal,
        decimal subTotal,
        decimal vatAmount)
    {
        InvoiceNumber = invoiceNumber;
        InvoiceDate = invoiceDate;
        DueDate = dueDate;
        PaymentDate = paymentDate;
        InvoiceTotal = invoiceTotal;
        SubTotal = subTotal;
        VatAmount = vatAmount;
    }

    public void SetCycle(Guid cycleId)
    {
        CycleId = cycleId;
    }

    public void SetFilePath(string path)
    {
        FilePath = path;
    }

    public void BookPayment(DateOnly paymentDate)
    {
        PaymentDate = paymentDate;
    }
}