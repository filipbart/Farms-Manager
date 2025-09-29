using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;

public interface IFeedPriceRepository : IRepository<FeedPriceEntity>
{
    Task<IEnumerable<FeedPriceEntity>> GetFeedPricesForInvoiceDateAsync(Guid farmId, Guid cycleId, string feedName,
        DateOnly invoiceDate);
}