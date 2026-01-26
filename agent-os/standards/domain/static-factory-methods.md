---
# Static Factory Methods

All entities use static factory methods instead of public constructors:

```csharp
public static FarmEntity CreateNew(string name, string producerNumber, string nip, string address,
    Guid? createdBy = null)
{
    return new FarmEntity
    {
        Name = name,
        ProducerNumber = producerNumber.Replace(" ", "").Trim(),
        Nip = nip.Replace("PL", "").Replace("-", "").Replace(" ", "").Trim(),
        Address = address,
        CreatedBy = createdBy
    };
}
```

- **Validation & Safety** - Factory methods validate input and enforce business rules
- **Default Values** - Set required properties and apply data transformations
- **Consistency** - All entities must have CreateNew() method, no exceptions
- **Protected Constructor** - Keep constructor protected to force factory usage
- **Optional Parameters** - Use nullable Guid for CreatedBy when creator is optional
