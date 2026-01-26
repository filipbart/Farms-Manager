---
# API Routing Conventions

Follow consistent routing patterns across all controllers:

```csharp
// Create
[HttpPost("add")]
public async Task<IActionResult> AddFarm(AddFarmCommand command)

// Read
[HttpGet]                           // List all
[HttpGet("{id:guid}")]               // Get single
[HttpGet("{id:guid}/subresource")]   // Get related

// Update  
[HttpPatch("update/{id:guid}")]
[HttpPut("resource/{id:guid}")]       // Some controllers use PUT

// Delete
[HttpPost("delete/{id:guid}")]        // Most common
[HttpDelete("{id:guid}/delete")]     // Some controllers use DELETE
[HttpDelete("delete/{id:guid}")]
```

- Prefer POST for delete operations (consistent across most controllers)
- Use PATCH for partial updates, PUT for full replacements
- Include action name in route (add, update, delete)
- Use `{id:guid}` for GUID parameters
