---
# Specification Pattern

Query building using Ardalis.Specification with BaseSpecification:

```csharp
public sealed class GetAllFarmsSpec : BaseSpecification<FarmEntity>
{
    public GetAllFarmsSpec(List<Guid> accessibleFarmIds, bool isAdmin, bool? showDeleted = null)
    {
        EnsureExists(showDeleted, isAdmin);
        if (accessibleFarmIds is not null && accessibleFarmIds.Count != 0)
        {
            Query.Where(t => accessibleFarmIds.Contains(t.Id));
        }

        Query.Include(t => t.Henhouses);
        Query.Include(t => t.ActiveCycle);
        Query.Include(t => t.TaxBusinessEntity);
        Query.OrderBy(t => t.Name);
    }
}

// Usage in query handler
public async Task<BaseResponse<GetAllFarmsQueryResponse>> Handle(GetAllFarmsQuery request, CancellationToken ct)
{
    var items = await _farmRepository.ListAsync<FarmRowDto>(
        new GetAllFarmsSpec(accessibleFarmIds, isAdmin), ct);
    return BaseResponse.CreateResponse(new GetAllFarmsQueryResponse { Items = items });
}
```

- **BaseSpecification Inheritance** - All specifications inherit from BaseSpecification<T>
- **Usually Specifications** - Query handlers typically use specifications (rare direct LINQ)
- **Complex Queries** - Specifications encapsulate complex query logic (includes, where, ordering)
- **Reusable** - Specifications can be reused across different handlers
- **Type Mapping** - Specifications can map to different result types (FarmRowDto)
- **Extension Methods** - Use EnsureExists() and other utility methods for common patterns
