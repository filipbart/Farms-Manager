---
# Handler Pattern

Consistent handler implementation with dependency injection:

```csharp
public class AddFarmCommandHandler : IRequestHandler<AddFarmCommand, EmptyBaseResponse>
{
    private readonly IFarmRepository _farmRepository;
    private readonly IUserDataResolver _userDataResolver;

    public AddFarmCommandHandler(IFarmRepository farmRepository, IUserDataResolver userDataResolver)
    {
        _farmRepository = farmRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(AddFarmCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var newFarm = FarmEntity.CreateNew(request.Name, request.ProdNumber, request.Nip, request.Address, userId);
        await _farmRepository.AddAsync(newFarm, cancellationToken);

        return new EmptyBaseResponse();
    }
}
```

- **IRequestHandler Interface** - All handlers implement IRequestHandler<TCommand, TResponse>
- **Constructor Injection** - Dependencies always injected through constructor (no other approaches)
- **Private Fields** - Dependencies stored as private readonly fields
- **Async Handle Method** - Handle method is always async with CancellationToken
- **Return Types** - Commands return EmptyBaseResponse, Queries return BaseResponse<T>
- **Exception Handling** - Throw DomainException for business rule violations
