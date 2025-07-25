using AutoMapper;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Infrastructure.Repositories.FeedAggregate;

public class FeedInvoiceCorrectionRepository : AbstractRepository<FeedInvoiceCorrectionEntity>,
    IFeedInvoiceCorrectionRepository
{
    private readonly FarmsManagerContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public FeedInvoiceCorrectionRepository(FarmsManagerContext context, IConfigurationProvider configurationProvider) :
        base(
            context, configurationProvider)
    {
        _context = context;
    }
}