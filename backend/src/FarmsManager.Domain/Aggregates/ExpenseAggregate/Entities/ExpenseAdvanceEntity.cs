using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Enums;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;

public class ExpenseAdvanceEntity : Entity
{
    public Guid EmployeeId { get; init; }
    public Guid? ExpenseAdvanceCategoryId { get; protected internal set; }
    public DateOnly Date { get; protected internal set; }
    public ExpenseAdvanceCategoryType Type { get; protected internal set; }
    public string Name { get; protected internal set; }
    public decimal Amount { get; protected internal set; }
    public string Comment { get; protected internal set; }
    public string FilePath { get; protected internal set; }
    public virtual EmployeeEntity Employee { get; init; }
    public virtual ExpenseAdvanceCategoryEntity ExpenseAdvanceCategory { get; init; }

    protected ExpenseAdvanceEntity()
    {
    }

    public static ExpenseAdvanceEntity CreateNew(
        Guid employeeId,
        Guid expenseAdvanceCategoryId,
        DateOnly date,
        ExpenseAdvanceCategoryType type,
        string name,
        decimal amount,
        string comment,
        string filePath,
        Guid? userId = null)
    {
        return new ExpenseAdvanceEntity
        {
            EmployeeId = employeeId,
            ExpenseAdvanceCategoryId = expenseAdvanceCategoryId,
            Date = date,
            Type = type,
            Name = name,
            Amount = amount,
            Comment = comment,
            FilePath = filePath,
            CreatedBy = userId
        };
    }

    public void Update(
        DateOnly date,
        ExpenseAdvanceCategoryType type,
        string name,
        decimal amount,
        string comment,
        string filePath)
    {
        Date = date;
        Type = type;
        Name = name;
        Amount = amount;
        Comment = comment;
        FilePath = filePath;
    }
    
    public void SetExpenseAdvanceCategory(Guid? expenseAdvanceCategoryId)
    {
        ExpenseAdvanceCategoryId = expenseAdvanceCategoryId;
    }
}