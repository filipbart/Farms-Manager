---
# Collection Encapsulation Pattern

Entity collections use private backing fields with read-only public access:

```csharp
private readonly List<HenhouseEntity> _henhouses = [];
private readonly List<EmployeeEntity> _employees = [];

public virtual IReadOnlyCollection<HenhouseEntity> Henhouses => _henhouses.AsReadOnly();
public virtual IReadOnlyCollection<EmployeeEntity> Employees => _employees.AsReadOnly();
```

- **Fluent API Pattern** - Collections are modified only through entity methods
- **Encapsulation** - Private fields prevent external direct modification
- **Read-Only Access** - External code can only read collections via IReadOnlyCollection
- **Business Methods** - Use AddXxx() methods to modify collections safely
- **All Collections** - Apply this pattern to all entity collections, no exceptions

```csharp
public void AddFile(EmployeeFileEntity file)
{
    _files.Add(file);
}
```
