using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;

namespace FarmsManager.Application.Specifications.Insertions;

public sealed class InsertionByIdSpec : BaseSpecification<InsertionEntity>, ISingleResultSpecification<InsertionEntity>
{
    public InsertionByIdSpec(Guid insertionId)
    {
        EnsureExists();
        Query.Where(t => t.Id == insertionId);
    }
}