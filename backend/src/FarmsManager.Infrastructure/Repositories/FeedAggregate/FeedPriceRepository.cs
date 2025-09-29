using AutoMapper;
using Dapper;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Infrastructure.Repositories.FeedAggregate;

public class FeedPriceRepository : AbstractRepository<FeedPriceEntity>, IFeedPriceRepository
{
    private readonly FarmsManagerContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public FeedPriceRepository(FarmsManagerContext context, IConfigurationProvider configurationProvider) : base(
        context, configurationProvider)
    {
        _context = context;
    }

    public async Task<IEnumerable<FeedPriceEntity>> GetFeedPricesForInvoiceDateAsync(Guid farmId, Guid cycleId,
        string feedName, DateOnly invoiceDate)
    {
        const string sql = """
                           WITH LatestPriceDate AS (SELECT price_date
                                                    FROM farms_manager.feed_price
                                                    WHERE farm_id = @farmId
                                                      AND cycle_id = @cycleId
                                                      AND Name = @feedName
                                                      AND price_date <= @invoiceDate
                                                      AND date_deleted_utc is null
                                                    ORDER BY price_date DESC
                                                    LIMIT 1)
                           SELECT *
                           FROM farms_manager.feed_price
                           WHERE farm_id = @farmId
                             AND cycle_id = @cycleId
                             AND Name = @feedName
                             AND date_deleted_utc is null
                             AND price_date = (SELECT price_date FROM LatestPriceDate);
                           """;

        var connection = _context.Database.GetDbConnection();
        return await connection.QueryAsync<FeedPriceEntity>(sql, new { farmId, cycleId, feedName, invoiceDate });
    }
}