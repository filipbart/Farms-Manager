---
# CQRS Pattern

Command Query Responsibility Segregation with MediatR:

```csharp
// Commands (write operations)
public record AddFarmCommand : IRequest<EmptyBaseResponse>
{
    public string Name { get; init; }
    public string ProdNumber { get; init; }
    public string Nip { get; init; }
    public string Address { get; init; }
}

public class AddFarmCommandHandler : IRequestHandler<AddFarmCommand, EmptyBaseResponse>
{
    public async Task<EmptyBaseResponse> Handle(AddFarmCommand request, CancellationToken ct)
    {
        // Business logic, validation, persistence
        var newFarm = FarmEntity.CreateNew(request.Name, request.ProdNumber, request.Nip, request.Address, userId);
        await _farmRepository.AddAsync(newFarm, ct);
        return new EmptyBaseResponse();
    }
}

// Queries (read operations)
public record GetAllFarmsQuery : IRequest<BaseResponse<GetAllFarmsQueryResponse>>;

public class GetAllFarmsQueryHandler : IRequestHandler<GetAllFarmsQuery, BaseResponse<GetAllFarmsQueryResponse>>
{
    public async Task<BaseResponse<GetAllFarmsQueryResponse>> Handle(GetAllFarmsQuery request, CancellationToken ct)
    {
        // Data retrieval, mapping, no mutations
        var items = await _farmRepository.ListAsync<FarmRowDto>(spec, ct);
        return BaseResponse.CreateResponse(new GetAllFarmsQueryResponse { Items = items });
    }
}
```

- **Commands for Write** - All write operations must be Commands (no exceptions)
- **Queries for Read** - Read operations use Queries, typically read-only (rare exceptions)
- **Separate Handlers** - Each command/query has dedicated handler class
- **MediatR Integration** - Uses IRequest<TResponse> and IRequestHandler<TCommand, TResponse>
- **Clear Separation** - Commands modify state, Queries return data
