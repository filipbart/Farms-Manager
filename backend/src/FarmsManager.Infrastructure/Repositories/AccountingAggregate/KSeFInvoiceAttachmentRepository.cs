using AutoMapper;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Infrastructure.Repositories.AccountingAggregate;

public class KSeFInvoiceAttachmentRepository : AbstractRepository<KSeFInvoiceAttachmentEntity>, IKSeFInvoiceAttachmentRepository
{
    public IUnitOfWork UnitOfWork { get; }

    public KSeFInvoiceAttachmentRepository(FarmsManagerContext context, IConfigurationProvider configurationProvider) : base(
        context,
        configurationProvider)
    {
        UnitOfWork = context;
    }
}
