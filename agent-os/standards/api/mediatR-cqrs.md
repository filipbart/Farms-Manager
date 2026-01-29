---
# MediatR CQRS Pattern

Controllers must only forward requests to MediatR - no business logic:

```csharp
public async Task<IActionResult> AddFarm(AddFarmCommand command)
{
    return Ok(await mediator.Send(command));
}

public async Task<IActionResult> GetFarm([FromRoute] Guid farmId)
{
    return Ok(await mediator.Send(new GetFarmQuery(farmId)));
}
```

- Controllers are thin - only HTTP concerns (routing, validation)
- All business logic in Command/Query handlers
- Commands for write operations, Queries for read operations
- No exceptions - every endpoint uses mediator.Send()
- Return Ok() with mediator response directly

```csharp
// Constructor injection
public class FarmsController(IMediator mediator) : BaseController
```
