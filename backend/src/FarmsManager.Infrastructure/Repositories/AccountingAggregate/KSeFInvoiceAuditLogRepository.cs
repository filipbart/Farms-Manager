using AutoMapper;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Infrastructure.Repositories.AccountingAggregate;

public class KSeFInvoiceAuditLogRepository : AbstractRepository<KSeFInvoiceAuditLogEntity>, IKSeFInvoiceAuditLogRepository
{
    public IUnitOfWork UnitOfWork { get; }

    public KSeFInvoiceAuditLogRepository(FarmsManagerContext context, IConfigurationProvider configurationProvider) : base(
        context,
        configurationProvider)
    {
        UnitOfWork = context;
    }
}
