using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;

public class ExpenseProductionEntity : Entity
{
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
    
    
}