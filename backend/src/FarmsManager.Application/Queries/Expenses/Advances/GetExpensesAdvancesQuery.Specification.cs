using Ardalis.Specification;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;

namespace FarmsManager.Application.Queries.Expenses.Advances;

public sealed class GetAllExpensesAdvancesSpec : BaseSpecification<ExpenseAdvanceEntity>
{
    public GetAllExpensesAdvancesSpec(Guid employeeId, GetExpensesAdvancesQueryFilters filters, bool withPagination,
        bool withFilters, bool isAdmin)
    {
        EnsureExists(filters.ShowDeleted, isAdmin);
        DisableTracking();

        if (withFilters)
        {
            PopulateFilters(filters);
            ApplyOrdering(filters);
        }

        Query.Where(t => t.EmployeeId == employeeId);
        Query.Include(t => t.ExpenseAdvanceCategory);

        if (withPagination)
        {
            Paginate(filters);
        }
    }

    private void PopulateFilters(GetExpensesAdvancesQueryFilters filters)
    {
        if (filters.Years != null && filters.Years.Count != 0)
        {
            Query.Where(ea => filters.Years.Contains(ea.Date.Year));
        }

        if (filters.Months != null && filters.Months.Count != 0)
        {
            Query.Where(ea => filters.Months.Contains(ea.Date.Month));
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