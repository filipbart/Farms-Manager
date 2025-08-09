using Ardalis.Specification;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;

namespace FarmsManager.Application.Queries.Hatcheries.Prices;

public sealed class GetAllHatcheryPricesSpec : BaseSpecification<HatcheryPriceEntity>
{
    public GetAllHatcheryPricesSpec(GetHatcheryPricesQueryFilters filters, bool withPagination)
    {
        EnsureExists();
        DisableTracking();

        PopulateFilters(filters);
        ApplyOrdering(filters);

        Query.Include(hp => hp.Hatchery);

        if (withPagination)
        {
            Paginate(filters);
        }
    }

    private void PopulateFilters(GetHatcheryPricesQueryFilters filters)
    {
        if (filters.HatcheryIds is not null && filters.HatcheryIds.Count != 0)
        {
            Query.Where(hp => filters.HatcheryIds.Contains(hp.HatcheryId));
        }

        if (filters.DateSince is not null)
        {
            Query.Where(hp => hp.Date >= filters.DateSince.Value);
        }

        if (filters.DateTo is not null)
        {
            Query.Where(hp => hp.Date <= filters.DateTo.Value);
        }

        if (filters.PriceFrom is not null)
        {
            Query.Where(hp => hp.Price >= filters.PriceFrom.Value);
        }

        if (filters.PriceTo is not null)
        {
            Query.Where(hp => hp.Price <= filters.PriceTo.Value);
        }
    }

    private void ApplyOrdering(GetHatcheryPricesQueryFilters filters)
    {
        var isDescending = filters.IsDescending;

        switch (filters.OrderBy)
        {
            case HatcheriesPricesOrderBy.HatcheryName:
                if (isDescending)
                {
                    Query.OrderByDescending(hp => hp.Hatchery.Name);
                }
                else
                {
                    Query.OrderBy(hp => hp.Hatchery.Name);
                }

                break;

            case HatcheriesPricesOrderBy.Price:
                if (isDescending)
                {
                    Query.OrderByDescending(hp => hp.Price);
                }
                else
                {
                    Query.OrderBy(hp => hp.Price);
                }

                break;

            case HatcheriesPricesOrderBy.Date:
            default:
                if (isDescending)
                {
                    Query.OrderByDescending(hp => hp.Date);
                }
                else
                {
                    Query.OrderBy(hp => hp.Date);
                }

                break;
        }
    }
}