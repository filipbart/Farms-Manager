using AutoMapper;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Infrastructure.Repositories.AccountingAggregate;

public class KSeFInvoiceRepository : AbstractRepository<KSeFInvoiceEntity>, IKSeFInvoiceRepository
{
    public IUnitOfWork UnitOfWork { get; }

    public KSeFInvoiceRepository(FarmsManagerContext context, IConfigurationProvider configurationProvider) : base(
        context,
        configurationProvider)
    {
        UnitOfWork = context;
    }
}
