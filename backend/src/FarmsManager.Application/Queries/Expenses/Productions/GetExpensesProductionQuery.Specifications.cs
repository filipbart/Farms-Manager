using Ardalis.Specification;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;

namespace FarmsManager.Application.Queries.Expenses.Productions;

public sealed class GetAllExpenseProductionsSpec : BaseSpecification<ExpenseProductionEntity>
{
    public GetAllExpenseProductionsSpec(GetExpensesProductionsFilters filters, bool withPagination)
    {
        EnsureExists();
        DisableTracking();

        PopulateFilters(filters);
        ApplyOrdering(filters);

        if (withPagination)
        {
            Paginate(filters);
        }
    }

    private void PopulateFilters(GetExpensesProductionsFilters filters)
    {
        if (filters.FarmIds is not null && filters.FarmIds.Any())
        {
            Query.Where(ep => filters.FarmIds.Contains(ep.FarmId));
        }

        if (filters.ContractorIds is not null && filters.ContractorIds.Any())
        {
            Query.Where(ep => filters.ContractorIds.Contains(ep.ContractorId));
        }

        if (filters.ExpensesTypeNames is not null && filters.ExpensesTypeNames.Any())
        {
            Query.Where(ep => filters.ExpensesTypeNames.Contains(ep.ExpenseType.Name));
        }

        if (filters.Cycles is not null && filters.Cycles.Any())
        {
        }

        if (filters.DateSince is not null)
        {
            Query.Where(ep => ep.InvoiceDate.Date >= filters.DateSince.Value);
        }

        if (filters.DateTo is not null)
        {
            Query.Where(ep => ep.InvoiceDate.Date <= filters.DateTo.Value);
        }
    }

    private void ApplyOrdering(GetExpensesProductionsFilters filters)
    {
        var isDescending = filters.IsDescending;

        switch (filters.OrderBy)
        {
            case ExpensesProductionsOrderBy.Cycle:
                if (isDescending)
                {
                    Query.OrderByDescending(ep => ep.Cycle.Identifier)
                        .ThenByDescending(ep => ep.Cycle.Year);
                }
                else
                {
                    Query.OrderBy(ep => ep.Cycle.Year)
                        .ThenBy(ep => ep.Cycle.Identifier);
                }

                break;

            case ExpensesProductionsOrderBy.Farm:
                if (isDescending)
                {
                    Query.OrderByDescending(ep => ep.Farm.Name);
                }
                else
                {
                    Query.OrderBy(ep => ep.Farm.Name);
                }

                break;

            case ExpensesProductionsOrderBy.Contractor:
                if (isDescending)
                {
                    Query.OrderByDescending(ep => ep.Contractor.Name);
                }
                else
                {
                    Query.OrderBy(ep => ep.Contractor.Name);
                }

                break;

            case ExpensesProductionsOrderBy.ExpenseType:
                if (isDescending)
                {
                    Query.OrderByDescending(ep => ep.ExpenseType.Name);
                }
                else
                {
                    Query.OrderBy(ep => ep.ExpenseType.Name);
                }

                break;

            case ExpensesProductionsOrderBy.InvoiceTotal:
                if (isDescending)
                {
                    Query.OrderByDescending(ep => ep.InvoiceTotal);
                }
                else
                {
                    Query.OrderBy(ep => ep.InvoiceTotal);
                }

                break;

            case ExpensesProductionsOrderBy.SubTotal:
                if (isDescending)
                {
                    Query.OrderByDescending(ep => ep.SubTotal);
                }
                else
                {
                    Query.OrderBy(ep => ep.SubTotal);
                }

                break;

            case ExpensesProductionsOrderBy.VatAmount:
                if (isDescending)
                {
                    Query.OrderByDescending(ep => ep.VatAmount);
                }
                else
                {
                    Query.OrderBy(ep => ep.VatAmount);
                }

                break;

            case ExpensesProductionsOrderBy.InvoiceDate:
                if (isDescending)
                {
                    Query.OrderByDescending(ep => ep.InvoiceDate);
                }
                else
                {
                    Query.OrderBy(ep => ep.InvoiceDate);
                }

                break;

            case ExpensesProductionsOrderBy.DateCreatedUtc:
            default:
                if (isDescending)
                {
                    Query.OrderByDescending(ep => ep.DateCreatedUtc);
                }
                else
                {
                    Query.OrderBy(ep => ep.DateCreatedUtc);
                }

                break;
        }
    }
}