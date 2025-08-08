using Ardalis.Specification;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;
using LinqKit;

namespace FarmsManager.Application.Queries.Sales.Invoices;

public sealed class GetAllSalesInvoicesSpec : BaseSpecification<SaleInvoiceEntity>
{
    public GetAllSalesInvoicesSpec(GetSalesInvoicesQueryFilters filters, bool withPagination)
    {
        EnsureExists();
        DisableTracking();

        Query.Include(t => t.Farm);
        Query.Include(t => t.Cycle);
        Query.Include(t => t.Slaughterhouse);

        PopulateFilters(filters);
        ApplyOrdering(filters);
        if (withPagination)
        {
            Paginate(filters);
        }
    }

    private void PopulateFilters(GetSalesInvoicesQueryFilters filters)
    {
        if (filters.FarmIds is not null && filters.FarmIds.Any())
        {
            Query.Where(t => filters.FarmIds.Contains(t.FarmId));
        }

        if (filters.SlaughterhouseIds is not null && filters.SlaughterhouseIds.Any())
        {
            Query.Where(t => filters.SlaughterhouseIds.Contains(t.SlaughterhouseId));
        }

        if (filters.CyclesDict is not null && filters.CyclesDict.Count != 0)
        {
            var predicate = PredicateBuilder.New<SaleInvoiceEntity>();

            predicate = filters.CyclesDict.Aggregate(predicate,
                (current, cycleFilter) => current.Or(t =>
                    t.Cycle.Identifier == cycleFilter.Identifier && t.Cycle.Year == cycleFilter.Year));

            Query.Where(predicate);
        }

        if (filters.DateSince is not null)
        {
            Query.Where(t => t.InvoiceDate >= filters.DateSince.Value);
        }

        if (filters.DateTo is not null)
        {
            Query.Where(t => t.InvoiceDate <= filters.DateTo.Value);
        }
    }

    private void ApplyOrdering(GetSalesInvoicesQueryFilters filters)
    {
        var isDescending = filters.IsDescending;
        switch (filters.OrderBy)
        {
            case SalesInvoicesOrderBy.Cycle:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.Cycle.Identifier).ThenByDescending(t => t.Cycle.Year);
                }
                else
                {
                    Query.OrderBy(t => t.Cycle.Year).ThenBy(t => t.Cycle.Identifier);
                }

                break;

            case SalesInvoicesOrderBy.Farm:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.Farm.Name);
                }
                else
                {
                    Query.OrderBy(t => t.Farm.Name);
                }

                break;

            case SalesInvoicesOrderBy.Slaughterhouse:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.Slaughterhouse.Name);
                }
                else
                {
                    Query.OrderBy(t => t.Slaughterhouse.Name);
                }

                break;

            case SalesInvoicesOrderBy.InvoiceNumber:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.InvoiceNumber);
                }
                else
                {
                    Query.OrderBy(t => t.InvoiceNumber);
                }

                break;

            case SalesInvoicesOrderBy.DueDate:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.DueDate);
                }
                else
                {
                    Query.OrderBy(t => t.DueDate);
                }

                break;

            case SalesInvoicesOrderBy.InvoiceTotal:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.InvoiceTotal);
                }
                else
                {
                    Query.OrderBy(t => t.InvoiceTotal);
                }

                break;

            case SalesInvoicesOrderBy.SubTotal:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.SubTotal);
                }
                else
                {
                    Query.OrderBy(t => t.SubTotal);
                }

                break;

            case SalesInvoicesOrderBy.VatAmount:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.VatAmount);
                }
                else
                {
                    Query.OrderBy(t => t.VatAmount);
                }

                break;

            case SalesInvoicesOrderBy.DateCreatedUtc:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.DateCreatedUtc);
                }
                else
                {
                    Query.OrderBy(t => t.DateCreatedUtc);
                }

                break;

            case SalesInvoicesOrderBy.InvoiceDate:
            default:
                if (isDescending)
                {
                    Query.OrderByDescending(t => t.InvoiceDate);
                }
                else
                {
                    Query.OrderBy(t => t.InvoiceDate);
                }

                break;
        }
    }
}