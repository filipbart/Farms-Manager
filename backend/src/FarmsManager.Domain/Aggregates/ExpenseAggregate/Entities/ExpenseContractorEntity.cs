using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;

public class ExpenseContractorEntity : Entity
{
    protected ExpenseContractorEntity()
    {
        ExpenseTypes = new List<ExpenseContractorExpenseTypeEntity>();
    }

    public string Name { get; protected internal set; }
    public string Nip { get; protected internal set; }
    public string Address { get; protected internal set; }
    
    public virtual ICollection<ExpenseContractorExpenseTypeEntity> ExpenseTypes { get; protected internal set; }

    public void AddExpenseType(Guid expenseTypeId, Guid? userId = null)
    {
        if (!ExpenseTypes.Any(et => et.ExpenseTypeId == expenseTypeId))
        {
            ExpenseTypes.Add(ExpenseContractorExpenseTypeEntity.CreateNew(Id, expenseTypeId, userId));
        }
    }

    public void RemoveExpenseType(Guid expenseTypeId)
    {
        var item = ExpenseTypes.FirstOrDefault(et => et.ExpenseTypeId == expenseTypeId);
        if (item != null)
        {
            ExpenseTypes.Remove(item);
        }
    }

    public void SetExpenseTypes(IEnumerable<Guid> expenseTypeIds, Guid? userId = null)
    {
        ExpenseTypes.Clear();
        foreach (var typeId in expenseTypeIds)
        {
            ExpenseTypes.Add(ExpenseContractorExpenseTypeEntity.CreateNew(Id, typeId, userId));
        }
    }

    public static ExpenseContractorEntity CreateNew(string name, string nip, string address,
        IEnumerable<Guid> expenseTypeIds, Guid? userId = null)
    {
        var entity = new ExpenseContractorEntity
        {
            Name = name,
            Nip = nip.Replace("PL", "").Replace("-", "").Replace(" ", "").Trim(),
            Address = address,
            CreatedBy = userId
        };
        
        foreach (var typeId in expenseTypeIds)
        {
            entity.ExpenseTypes.Add(ExpenseContractorExpenseTypeEntity.CreateNew(entity.Id, typeId, userId));
        }
        
        return entity;
    }

    public static ExpenseContractorEntity CreateNewFromInvoice(string name, string nip, string address,
        Guid? userId = null)
    {
        return new ExpenseContractorEntity
        {
            Name = name,
            Nip = nip.Replace("PL", "").Replace("-", "").Replace(" ", "").Trim(),
            Address = address,
            CreatedBy = userId
        };
    }

    public void Update(string name, string nip, string address)
    {
        Name = name;
        Nip = nip.Replace("PL", "").Replace("-", "").Replace(" ", "").Trim();
        Address = address;
    }
}