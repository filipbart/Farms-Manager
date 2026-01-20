using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;

namespace FarmsManager.Application.Specifications.TaxBusinessEntities;

public sealed class AllActiveTaxBusinessEntitiesSpec : BaseSpecification<TaxBusinessEntity>
{
    public AllActiveTaxBusinessEntitiesSpec()
    {
        DisableTracking();
        EnsureExists();
    }
}
