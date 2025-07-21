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

    public string InvoiceNumber { get; protected internal set; }
    public string BankAccountNumber { get; protected internal set; }
    public string VendorName { get; protected internal set; }
    public string ItemName { get; protected internal set; }
    public decimal Quantity { get; protected internal set; }
    public decimal UnitPrice { get; protected internal set; }
    public DateOnly InvoiceDate { get; protected internal set; }
    public DateOnly DueDate { get; protected internal set; }
    public decimal InvoiceTotal { get; protected internal set; }
    public decimal SubTotal { get; protected internal set; }
    public decimal VatAmount { get; protected internal set; }
    public string Comment { get; protected internal set; }

    public virtual CycleEntity Cycle { get; init; }
    public virtual HenhouseEntity Henhouse { get; init; }
    public virtual FarmEntity Farm { get; init; }

    public string FilePath { get; protected internal set; }
    public decimal? CorrectUnitPrice { get; protected internal set; }
    public DateTime? PaymentDateUtc { get; protected internal set; }

    public void SetCorrectUnitPrice(decimal correctUnitPrice) => CorrectUnitPrice = correctUnitPrice;
    public void MarkAsPaid() => PaymentDateUtc = DateTime.UtcNow;

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

    public void Update(
        string invoiceNumber,
        string bankAccountNumber,
        string itemName,
        string vendorName,
        decimal quantity,
        decimal unitPrice,
        DateOnly invoiceDate,
        DateOnly dueDate,
        decimal invoiceTotal,
        decimal subTotal,
        decimal vatAmount,
        string comment)
    {
        InvoiceNumber = invoiceNumber;
        BankAccountNumber = bankAccountNumber;
        ItemName = itemName;
        VendorName = vendorName;
        Quantity = quantity;
        UnitPrice = unitPrice;
        InvoiceDate = invoiceDate;
        DueDate = dueDate;
        InvoiceTotal = invoiceTotal;
        SubTotal = subTotal;
        VatAmount = vatAmount;
        Comment = comment;
    }
}