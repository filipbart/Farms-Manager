using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;

namespace FarmsManager.Application.Specifications.TaxBusinessEntities;

public sealed class TaxBusinessEntityByIdSpec : SingleResultSpecification<TaxBusinessEntity>
{
    public TaxBusinessEntityByIdSpec(Guid id)
    {
        Query.Where(t => t.Id == id && t.DateDeletedUtc.HasValue == false);
    }
}
