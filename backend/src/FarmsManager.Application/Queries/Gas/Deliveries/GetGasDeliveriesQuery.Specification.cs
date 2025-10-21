using Ardalis.Specification;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;

namespace FarmsManager.Application.Queries.Gas.Deliveries;

public sealed class GetAllGasDeliveriesSpec : BaseSpecification<GasDeliveryEntity>
{
    public GetAllGasDeliveriesSpec(GetGasDeliveriesQueryFilters filters, bool withPagination,
        List<Guid> accessibleFarmIds, bool isAdmin)
    {
        EnsureExists(filters.ShowDeleted, isAdmin);
        DisableTracking();

        PopulateFilters(filters);
        ApplyOrdering(filters);

        Query.Include(t => t.Farm);
        Query.Include(t => t.GasContractor);
        Query.Include(t => t.Creator);
        Query.Include(t => t.Modifier);
        Query.Include(t => t.Deleter);

        if (accessibleFarmIds is not null && accessibleFarmIds.Count != 0)
            Query.Where(p => accessibleFarmIds.Contains(p.FarmId));

        if (withPagination)
        {
            Paginate(filters);
        }
    }

    private void PopulateFilters(GetGasDeliveriesQueryFilters filters)
    {
        if (filters.FarmIds is not null && filters.FarmIds.Count != 0)
        {
            Query.Where(gd => filters.FarmIds.Contains(gd.FarmId));
        }

        if (filters.ContractorIds is not null && filters.ContractorIds.Count != 0)
        {
            Query.Where(gd => filters.ContractorIds.Contains(gd.GasContractorId));
        }

        if (filters.DateSince is not null)
        {
            Query.Where(gd => gd.InvoiceDate >= filters.DateSince.Value);
        }

        if (filters.DateTo is not null)
        {
            Query.Where(gd => gd.InvoiceDate <= filters.DateTo.Value);
        }
    }

    private void ApplyOrdering(GetGasDeliveriesQueryFilters filters)
    {
        var isDescending = filters.IsDescending;

        switch (filters.OrderBy)
        {
            case GasDeliveriesOrderBy.Farm:
                if (isDescending)
                {
                    Query.OrderByDescending(gd => gd.Farm.Name);
                }
                else
                {
                    Query.OrderBy(gd => gd.Farm.Name);
                }

                break;

            case GasDeliveriesOrderBy.Contractor:
                if (isDescending)
                {
                    Query.OrderByDescending(gd => gd.GasContractor.Name);
                }
                else
                {
                    Query.OrderBy(gd => gd.GasContractor.Name);
                }

                break;

            case GasDeliveriesOrderBy.InvoiceNumber:
                if (isDescending)
                {
                    Query.OrderByDescending(gd => gd.InvoiceNumber);
                }
                else
                {
                    Query.OrderBy(gd => gd.InvoiceNumber);
                }

                break;

            case GasDeliveriesOrderBy.UnitPrice:
                if (isDescending)
                {
                    Query.OrderByDescending(gd => gd.UnitPrice);
                }
                else
                {
                    Query.OrderBy(gd => gd.UnitPrice);
                }

                break;

            case GasDeliveriesOrderBy.Quantity:
                if (isDescending)
                {
                    Query.OrderByDescending(gd => gd.Quantity);
                }
                else
                {
                    Query.OrderBy(gd => gd.Quantity);
                }

                break;

            case GasDeliveriesOrderBy.InvoiceDate:
            default:
                if (isDescending)
                {
                    Query.OrderByDescending(gd => gd.InvoiceDate);
                }
                else
                {
                    Query.OrderBy(gd => gd.InvoiceDate);
                }

                break;
        }
    }
}