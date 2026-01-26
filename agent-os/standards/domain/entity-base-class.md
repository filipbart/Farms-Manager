---
# Entity Base Class

All domain entities inherit from Entity base class:

```csharp
public abstract class Entity
{
    [Key] public Guid Id { get; private set; } = Guid.NewGuid();
    public DateTime DateCreatedUtc { get; private set; }
    public DateTime? DateModifiedUtc { get; private set; }
    public DateTime? DateDeletedUtc { get; private set; }
    
    public Guid? CreatedBy { get; set; }  // Nullable for system-created entities
    public Guid? ModifiedBy { get; private set; }
    public Guid? DeletedBy { get; private set; }
    
    [NotMapped] public virtual UserEntity Creator { get; set; }
    [NotMapped] public virtual UserEntity Modifier { get; set; }
    [NotMapped] public virtual UserEntity Deleter { get; set; }
}
```

- Id is auto-generated GUID, private set
- Audit fields track creation/modification/deletion timestamps
- User tracking fields are nullable for system-created entities (e.g., jobs)
- Navigation properties are [NotMapped] - loaded via joins when needed
- Provides SetModified() and Delete() methods
