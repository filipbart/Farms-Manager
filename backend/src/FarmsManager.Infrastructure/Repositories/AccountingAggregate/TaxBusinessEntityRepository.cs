using AutoMapper;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Infrastructure.Repositories.AccountingAggregate;

public class TaxBusinessEntityRepository : AbstractRepository<TaxBusinessEntity>, ITaxBusinessEntityRepository
{
    public IUnitOfWork UnitOfWork { get; }

    public TaxBusinessEntityRepository(FarmsManagerContext context, IConfigurationProvider configurationProvider) : base(
        context,
        configurationProvider)
    {
        UnitOfWork = context;
    }
}
