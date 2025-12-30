using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;

public class ExpenseContractorExpenseTypeEntity : Entity
{
    protected ExpenseContractorExpenseTypeEntity()
    {
    }

    public Guid ExpenseContractorId { get; protected internal set; }
    public Guid ExpenseTypeId { get; protected internal set; }
    
    public virtual ExpenseContractorEntity ExpenseContractor { get; protected internal set; }
    public virtual ExpenseTypeEntity ExpenseType { get; protected internal set; }

    public static ExpenseContractorExpenseTypeEntity CreateNew(
        Guid expenseContractorId,
        Guid expenseTypeId,
        Guid? userId = null)
    {
        return new ExpenseContractorExpenseTypeEntity
        {
            ExpenseContractorId = expenseContractorId,
            ExpenseTypeId = expenseTypeId,
            CreatedBy = userId
        };
    }
}
