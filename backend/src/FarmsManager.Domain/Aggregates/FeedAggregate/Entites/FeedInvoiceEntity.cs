using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.FeedAggregate.Entites;

public class FeedInvoiceEntity : Entity
{
    protected FeedInvoiceEntity()
    {
    }

    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public Guid HenhouseId { get; init; }

    public string InvoiceNumber { get; init; }
    public string BankAccountNumber { get; init; }
    public string VendorName { get; init; }
    public string ItemName { get; init; }
    public decimal Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public DateOnly InvoiceDate { get; init; }
    public DateOnly DueDate { get; init; }
    public decimal InvoiceTotal { get; init; }
    public decimal SubTotal { get; init; }
    public decimal VatAmount { get; init; }
    public string Comment { get; init; }

    public virtual CycleEntity Cycle { get; init; }
    public virtual HenhouseEntity Henhouse { get; init; }
    public virtual FarmEntity Farm { get; init; }

    public string FilePath { get; protected internal set; }


    public static FeedInvoiceEntity CreateNew(
        Guid farmId,
        Guid cycleId,
        Guid henhouseId,
        string invoiceNumber,
        string bankAccountNumber,
        string vendorName,
        string itemName,
        decimal quantity,
        decimal unitPrice,
        DateOnly invoiceDate,
        DateOnly dueDate,
        decimal invoiceTotal,
        decimal subTotal,
        decimal vatAmount,
        string comment,
        Guid? userId = null)
    {
        return new FeedInvoiceEntity
        {
            FarmId = farmId,
            CycleId = cycleId,
            HenhouseId = henhouseId,
            InvoiceNumber = invoiceNumber,
            BankAccountNumber = bankAccountNumber,
            VendorName = vendorName,
            ItemName = itemName,
            Quantity = quantity,
            UnitPrice = unitPrice,
            InvoiceDate = invoiceDate,
            DueDate = dueDate,
            InvoiceTotal = invoiceTotal,
            SubTotal = subTotal,
            VatAmount = vatAmount,
            Comment = comment,
            CreatedBy = userId
        };
    }

    public void SetFilePath(string filePath) => FilePath = filePath;
}