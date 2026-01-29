---
# Property Access Control

Entity properties use controlled access patterns:

```csharp
// Mutable within aggregate, immutable externally
public string Name { get; protected internal set; }

// Immutable after creation
public Guid FarmId { get; init; }

// Completely immutable (rare, only for special cases)
public Guid Id { get; private set; }
```

- **`protected internal set`** - Can be modified by entity methods and same-assembly code
- **`init`** - Set only during object initialization, immutable afterwards
- **`private set`** - Only accessible within the entity itself (rare usage)

Choice depends on how the property will be edited:
- Use `protected internal set` for properties modified by business methods
- Use `init` for properties that never change after creation
- Use `private set` only for special cases like Id
