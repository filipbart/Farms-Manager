using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;

public class ExpenseContractorEntity : Entity
{
    public Guid? ExpenseTypeId { get; protected internal set; }
    public string Name { get; protected internal set; }
    public string Nip { get; protected internal set; }
    public string Address { get; protected internal set; }
    public virtual ExpenseTypeEntity ExpenseType { get; protected internal set; }

    public void SetExpenseType(Guid? expenseTypeId)
    {
        ExpenseTypeId = expenseTypeId;
    }

    public static ExpenseContractorEntity CreateNew(Guid expenseTypeId, string name, string nip, string address,
        Guid? userId = null)
    {
        return new ExpenseContractorEntity
        {
            ExpenseTypeId = expenseTypeId,
            Name = name,
            Nip = nip.Replace("-", ""),
            Address = address,
            CreatedBy = userId
        };
    }

    public void Update(Guid expenseTypeId, string name, string nip, string address)
    {
        ExpenseTypeId = expenseTypeId;
        Name = name;
        Nip = nip.Replace("-", "");
        Address = address;
    }
}