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

        Query.Include(t => t.ExpenseContractor).ThenInclude(t => t.ExpenseType);

        if (withPagination)
        {
            Paginate(filters);
        }
    }

    private void PopulateFilters(GetExpensesProductionsFilters filters)
    {
        if (filters.FarmIds is not null && filters.FarmIds.Count != 0)
        {
            Query.Where(ep => filters.FarmIds.Contains(ep.FarmId));
        }

        if (filters.ContractorIds is not null && filters.ContractorIds.Count != 0)
        {
            Query.Where(ep => filters.ContractorIds.Contains(ep.ExpenseContractorId));
        }

        if (filters.ExpensesTypesIds is not null && filters.ExpensesTypesIds.Count != 0)
        {
            Query.Where(ep =>
                ep.ExpenseContractor.ExpenseTypeId.HasValue &&
                filters.ExpensesTypesIds.Contains(ep.ExpenseContractor.ExpenseTypeId.Value));
        }

        if (filters.Cycles is not null && filters.Cycles.Count != 0)
        {
        }

        if (filters.DateSince is not null)
        {
            Query.Where(ep => ep.InvoiceDate >= filters.DateSince.Value);
        }

        if (filters.DateTo is not null)
        {
            Query.Where(ep => ep.InvoiceDate <= filters.DateTo.Value);
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
                    Query.OrderByDescending(ep => ep.ExpenseContractor.Name);
                }
                else
                {
                    Query.OrderBy(ep => ep.ExpenseContractor.Name);
                }

                break;

            case ExpensesProductionsOrderBy.ExpenseType:
                if (isDescending)
                {
                    Query.OrderByDescending(ep => ep.ExpenseContractor.ExpenseType.Name);
                }
                else
                {
                    Query.OrderBy(ep => ep.ExpenseContractor.ExpenseType.Name);
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