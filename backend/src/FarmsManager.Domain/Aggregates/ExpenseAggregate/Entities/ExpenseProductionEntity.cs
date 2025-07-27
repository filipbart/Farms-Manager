using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;

public class ExpenseProductionEntity : Entity
{
    protected ExpenseProductionEntity()
    {
    }

    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public Guid ExpenseContractorId { get; protected internal set; }
    public string InvoiceNumber { get; protected internal set; }
    public decimal InvoiceTotal { get; protected internal set; }
    public decimal SubTotal { get; protected internal set; }
    public decimal VatAmount { get; protected internal set; }
    public DateOnly InvoiceDate { get; protected internal set; }
    public string FilePath { get; protected internal set; }
    public virtual FarmEntity Farm { get; set; }
    public virtual CycleEntity Cycle { get; set; }
    public virtual ExpenseContractorEntity ExpenseContractor { get; set; }

    public void SetFilePath(string filePath) => FilePath = filePath;

    public static ExpenseProductionEntity CreateNew(
        Guid farmId,
        Guid cycleId,
        Guid expenseContractorId,
        string invoiceNumber,
        decimal invoiceTotal,
        decimal subTotal,
        decimal vatAmount,
        DateOnly invoiceDate,
        Guid? userId = null)
    {
        return new ExpenseProductionEntity
        {
            FarmId = farmId,
            CycleId = cycleId,
            ExpenseContractorId = expenseContractorId,
            InvoiceNumber = invoiceNumber,
            InvoiceTotal = invoiceTotal,
            SubTotal = subTotal,
            VatAmount = vatAmount,
            InvoiceDate = invoiceDate,
            CreatedBy = userId
        };
    }

    public void Update(
        string invoiceNumber,
        decimal invoiceTotal,
        decimal subTotal,
        decimal vatAmount,
        DateOnly invoiceDate)
    {
        InvoiceNumber = invoiceNumber;
        InvoiceTotal = invoiceTotal;
        SubTotal = subTotal;
        VatAmount = vatAmount;
        InvoiceDate = invoiceDate;
    }
}