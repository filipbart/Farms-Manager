using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;

public class ExpenseProductionEntity : Entity
{
    protected ExpenseProductionEntity()
    {
    }

    public Guid FarmId { get; protected internal set; }
    public Guid CycleId { get; protected internal set; }
    public Guid ExpenseContractorId { get; protected internal set; }
    public Guid ExpenseTypeId { get; protected internal set; }
    public string InvoiceNumber { get; protected internal set; }
    public decimal InvoiceTotal { get; protected internal set; }
    public decimal SubTotal { get; protected internal set; }
    public decimal VatAmount { get; protected internal set; }
    public DateOnly InvoiceDate { get; protected internal set; }
    public string FilePath { get; protected internal set; }
    public string Comment { get; protected internal set; }
    public virtual FarmEntity Farm { get; set; }
    public virtual CycleEntity Cycle { get; set; }
    public virtual ExpenseContractorEntity ExpenseContractor { get; set; }
    public virtual ExpenseTypeEntity ExpenseType { get; set; }

    public void SetFilePath(string filePath) => FilePath = filePath;

    public static ExpenseProductionEntity CreateNew(
        Guid farmId,
        Guid cycleId,
        Guid expenseContractorId,
        Guid expenseTypeId,
        string invoiceNumber,
        decimal invoiceTotal,
        decimal subTotal,
        decimal vatAmount,
        DateOnly invoiceDate,
        string comment,
        Guid? userId = null)
    {
        return new ExpenseProductionEntity
        {
            FarmId = farmId,
            CycleId = cycleId,
            ExpenseContractorId = expenseContractorId,
            ExpenseTypeId = expenseTypeId,
            InvoiceNumber = invoiceNumber,
            InvoiceTotal = invoiceTotal,
            SubTotal = subTotal,
            VatAmount = vatAmount,
            InvoiceDate = invoiceDate,
            Comment = comment,
            CreatedBy = userId
        };
    }


    public void SetFarm(Guid farmId) => FarmId = farmId;
    public void SetCycle(Guid cycleId) => CycleId = cycleId;
    public void SetExpenseContractor(Guid expenseContractorId) => ExpenseContractorId = expenseContractorId;
    public void SetExpenseType(Guid expenseTypeId) => ExpenseTypeId = expenseTypeId;

    public void Update(
        string invoiceNumber,
        decimal invoiceTotal,
        decimal subTotal,
        decimal vatAmount,
        DateOnly invoiceDate,
        string comment)
    {
        InvoiceNumber = invoiceNumber;
        InvoiceTotal = invoiceTotal;
        SubTotal = subTotal;
        VatAmount = vatAmount;
        InvoiceDate = invoiceDate;
        Comment = comment;
    }
}