using Ardalis.Specification;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;

namespace FarmsManager.Application.Queries.Expenses.Advances;

public sealed class GetAllExpensesAdvancesSpec : BaseSpecification<ExpenseAdvanceEntity>
{
    public GetAllExpensesAdvancesSpec(Guid employeeId, GetExpensesAdvancesQueryFilters filters, bool withPagination)
    {
        EnsureExists();
        DisableTracking();

        PopulateFilters(filters);
        ApplyOrdering(filters);

        Query.Where(t => t.EmployeeId == employeeId);
        Query.Include(t => t.ExpenseAdvanceCategory);

        if (withPagination)
        {
            Paginate(filters);
        }
    }

    private void PopulateFilters(GetExpensesAdvancesQueryFilters filters)
    {
        if (filters.DateSince is not null)
        {
            Query.Where(ea => ea.Date >= filters.DateSince.Value);
        }

        if (filters.DateTo is not null)
        {
            Query.Where(ea => ea.Date <= filters.DateTo.Value);
        }
    }

    private void ApplyOrdering(GetExpensesAdvancesQueryFilters filters)
    {
        var isDescending = filters.IsDescending;

        switch (filters.OrderBy)
        {
            case ExpensesAdvancesOrderBy.Type:
                if (isDescending)
                {
                    Query.OrderByDescending(ea => ea.Type);
                }
                else
                {
                    Query.OrderBy(ea => ea.Type);
                }

                break;

            case ExpensesAdvancesOrderBy.Name:
                if (isDescending)
                {
                    Query.OrderByDescending(ea => ea.Name);
                }
                else
                {
                    Query.OrderBy(ea => ea.Name);
                }

                break;

            case ExpensesAdvancesOrderBy.Amount:
                if (isDescending)
                {
                    Query.OrderByDescending(ea => ea.Amount);
                }
                else
                {
                    Query.OrderBy(ea => ea.Amount);
                }

                break;

            case ExpensesAdvancesOrderBy.DateCreatedUtc:
                if (isDescending)
                {
                    Query.OrderByDescending(ea => ea.DateCreatedUtc);
                }
                else
                {
                    Query.OrderBy(ea => ea.DateCreatedUtc);
                }

                break;

            case ExpensesAdvancesOrderBy.Date:
            default:
                if (isDescending)
                {
                    Query.OrderByDescending(ea => ea.Date);
                }
                else
                {
                    Query.OrderBy(ea => ea.Date);
                }

                break;
        }
    }
}