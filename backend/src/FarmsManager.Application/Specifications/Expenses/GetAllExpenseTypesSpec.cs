using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;

namespace FarmsManager.Application.Specifications.Expenses;

public sealed class GetAllExpenseTypesSpec : BaseSpecification<ExpenseTypeEntity>
{
    public GetAllExpenseTypesSpec()
    {
        EnsureExists();
    }
}