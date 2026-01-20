using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;

namespace FarmsManager.Application.Specifications.TaxBusinessEntities;

public sealed class TaxBusinessEntityByIdSpec : BaseSpecification<TaxBusinessEntity>,
    ISingleResultSpecification<TaxBusinessEntity>
{
    public TaxBusinessEntityByIdSpec(Guid id)
    {
        EnsureExists();
        Query.Where(t => t.Id == id);
    }
}
