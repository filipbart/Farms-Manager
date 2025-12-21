using AutoMapper;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Infrastructure.Repositories.AccountingAggregate;

public class KSeFInvoiceRelationRepository : AbstractRepository<KSeFInvoiceRelationEntity>, IKSeFInvoiceRelationRepository
{
    public IUnitOfWork UnitOfWork { get; }

    public KSeFInvoiceRelationRepository(FarmsManagerContext context, IConfigurationProvider configurationProvider) 
        : base(context, configurationProvider)
    {
        UnitOfWork = context;
    }
}
