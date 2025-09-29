using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;

namespace FarmsManager.Application.Specifications.Feeds;

public sealed class GetFeedsInvoicesByDateRangeAndNameSpec : BaseSpecification<FeedInvoiceEntity>
{
    public GetFeedsInvoicesByDateRangeAndNameSpec(Guid farmId, Guid cycleId, DateOnly startDate, DateOnly endDate,
        string feedName)
    {
        EnsureExists();
        Query
            .Where(i => i.FarmId == farmId)
            .Where(i => i.CycleId == cycleId)
            .Where(i => i.ItemName == feedName)
            .Where(i => i.InvoiceDate >= startDate)
            .Where(i => i.InvoiceDate < endDate);
    }
}

public sealed class GetNextFeedPriceSpec : BaseSpecification<FeedPriceEntity>,
    ISingleResultSpecification<FeedPriceEntity>
{
    public GetNextFeedPriceSpec(Guid farmId, Guid cycleId, string feedName, DateOnly currentDate)
    {
        EnsureExists();
        Query
            .Where(p => p.FarmId == farmId)
            .Where(p => p.CycleId == cycleId)
            .Where(p => p.Name == feedName)
            .Where(p => p.PriceDate > currentDate)
            .OrderBy(p => p.PriceDate);
    }
}