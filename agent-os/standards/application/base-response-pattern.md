---
# BaseResponse Pattern

Consistent response wrapping across all handlers:

```csharp
// Commands without return data
public async Task<EmptyBaseResponse> Handle(AddFarmCommand request, CancellationToken ct)
{
    // Business logic
    await _farmRepository.AddAsync(newFarm, ct);
    return new EmptyBaseResponse();  // Or new EmptyBaseResponse()
}

// Queries with return data
public async Task<BaseResponse<GetAllFarmsQueryResponse>> Handle(GetAllFarmsQuery request, CancellationToken ct)
{
    var items = await _farmRepository.ListAsync<FarmRowDto>(spec, ct);
    return BaseResponse.CreateResponse(new GetAllFarmsQueryResponse { Items = items });
}

// Alternative constructor usage
return new BaseResponse<AuthenticateCommandResponse>
{
    ResponseData = new AuthenticateCommandResponse(token, expiry),
    Success = true
};
```

- **EmptyBaseResponse** - Used only for commands without return data
- **BaseResponse<T>** - Used for queries and commands returning data
- **CreateResponse()** - Standard factory method (constructor also acceptable)
- **Consistent Structure** - All responses follow BaseResponse envelope pattern
- **Success Property** - Computed property based on errors count
- **ResponseData** - Contains actual response payload for typed responses
