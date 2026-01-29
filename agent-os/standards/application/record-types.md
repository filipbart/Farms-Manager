---
# Record Types Pattern

All commands and queries use C# records with MediatR interfaces:

```csharp
// Commands with init properties (preferred)
public record AddFarmCommand : IRequest<EmptyBaseResponse>
{
    public string Name { get; init; }
    public string ProdNumber { get; init; }
    public string Nip { get; init; }
    public string Address { get; init; }
}

// Queries with init/set properties (case-dependent)
public record GetAllFarmsQuery : IRequest<BaseResponse<GetAllFarmsQueryResponse>>;

public record AuthenticateCommand : IRequest<BaseResponse<AuthenticateCommandResponse>>
{
    public string Login { get; set; }  // Sometimes set for flexibility
    public string Password { get; set; }
}

// Response DTOs
public record GetAllFarmsQueryResponse : PaginationModel<FarmRowDto>;
public record AuthenticateCommandResponse(string AccessToken, DateTime ExpiryAtUtc, bool MustChangePassword = false);
```

- **Records Preferred** - Almost all Commands/Queries use records (rare exceptions)
- **IRequest<TResponse>** - All implement MediatR request interface
- **init vs set** - Use `init` for immutability when possible, `set` when flexibility needed
- **Immutable by Default** - Records provide built-in equality and immutability
- **Response Records** - Response DTOs also use records for consistency
