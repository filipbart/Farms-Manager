---
# BaseController Pattern

All API controllers inherit from BaseController:

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public abstract class BaseController : ControllerBase;
```

- Every controller is secured by default (no public endpoints)
- No exceptions - all controllers inherit from BaseController
- `[ApiController]` enables automatic model validation
- Route convention: `/api/[controller]` (e.g., `/api/Farms`)

```csharp
public class FarmsController(IMediator mediator) : BaseController
{
    // All endpoints are automatically authorized
}
```
