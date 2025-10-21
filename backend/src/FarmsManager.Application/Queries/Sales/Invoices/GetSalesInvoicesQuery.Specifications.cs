using Ardalis.Specification;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;
using LinqKit;

namespace FarmsManager.Application.Queries.Sales.Invoices;

public sealed class GetAllSalesInvoicesSpec : BaseSpecification<SaleInvoiceEntity>
{
    public GetAllSalesInvoicesSpec(GetSalesInvoicesQueryFilters filters, bool withPagination,
        List<Guid> accessibleFarmIds)
    {
        EnsureExists();
        DisableTracking();

        Query.Include(t => t.Farm);
        Query.Include(t => t.Cycle);
        Query.Include(t => t.Slaughterhouse);

        if (accessibleFarmIds is not null && accessibleFarmIds.Count != 0)
            Query.Where(p => accessibleFarmIds.Contains(p.FarmId));

        PopulateFilters(filters);
        ApplyOrdering(filters);
        if (withPagination)
        {
            Paginate(filters);
        }
    }

    private void PopulateFilters(GetSalesInvoicesQueryFilters filters)
    {
        if (filters.FarmIds is not null && filters.FarmIds.Count != 0)
        {
            Query.Where(t => filters.FarmIds.Contains(t.FarmId));
        }

        if (filters.SlaughterhouseIds is not null && filters.SlaughterhouseIds.Count != 0)
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
            case SalesInvoicesOrderBy.Priority:
                // Multi-level sorting: Priority presence → Priority level → DueDate
                // Logika priorytetu:
                // - High (1): przeterminowane (DueDate < Today)
                // - Medium (2): 0-7 dni do terminu
                // - Low (3): 8-14 dni do terminu
                // - Brak (4): opłacone lub >14 dni do terminu
                Query.OrderByDescending(t => t.PaymentDate == null)
                    .ThenBy(t => 
                        t.PaymentDate != null ? 4 :
                        t.DueDate < DateOnly.FromDateTime(DateTime.Now) ? 1 :
                        t.DueDate.DayNumber - DateOnly.FromDateTime(DateTime.Now).DayNumber <= 7 ? 2 :
                        t.DueDate.DayNumber - DateOnly.FromDateTime(DateTime.Now).DayNumber <= 14 ? 3 : 4)
                    .ThenBy(t => t.DueDate);
                break;
            

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