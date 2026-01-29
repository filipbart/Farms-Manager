---
# Entity Business Methods

Entities contain business logic methods that modify internal state:

```csharp
public void Update(string name, string producerNumber, string nip, string address, Guid? taxBusinessEntityId = null)
{
    Name = name;
    ProducerNumber = producerNumber.Replace(" ", "").Trim();
    Nip = nip.Replace("PL", "").Replace("-", "").Replace(" ", "").Trim();
    Address = address;
    TaxBusinessEntityId = taxBusinessEntityId;
}

public void SetLatestCycle(CycleEntity cycle)
{
    _cycles.Add(cycle);
    ActiveCycle = cycle;
    ActiveCycleId = cycle.Id;
}
```

- **Void Return** - Business methods return void, modify entity state directly
- **Validation** - Methods can validate input and enforce business rules
- **State Changes** - Methods encapsulate complex state transitions
- **Collection Methods** - Use AddXxx(), RemoveXxx(), SetXxx() for collection management
- **No Exceptions** - Methods typically don't throw exceptions (validation handled elsewhere)
