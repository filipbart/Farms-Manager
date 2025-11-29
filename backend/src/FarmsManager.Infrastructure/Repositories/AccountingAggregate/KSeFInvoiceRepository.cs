using AutoMapper;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Infrastructure.Repositories.AccountingAggregate;

public class KSeFInvoiceRepository : AbstractRepository<KSeFInvoiceEntity>, IKSeFInvoiceRepository
{
    private readonly FarmsManagerContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public KSeFInvoiceRepository(FarmsManagerContext context, IConfigurationProvider configurationProvider) : base(
        context,
        configurationProvider)
    {
        _context = context;
    }
}
