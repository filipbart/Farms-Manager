---
# Repository Pattern with Specifications

Use Ardalis.Specification for query building and entity access:

```csharp
public interface IRepository<T> : IRepositoryBase<T>, ICustomRepository<T> where T : Entity
{
    IUnitOfWork UnitOfWork { get; }
}

public interface ICustomRepository<T>
{
    Task<List<TResult>> ListAsync<TResult>(ISpecification<T> spec);
    Task<T> GetAsync(ISingleResultSpecification<T> spec); // Throws RecordNotFoundException
    Task<TResult> GetAsync<TResult>(ISingleResultSpecification<T> spec);
    Task<TResult> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<T> spec);
    Task<TResult> FirstOrDefaultAsync<TResult>(ISingleResultSpecification<T> spec);
    void Update(T entity);
    void Add(T entity);
}
```

- **Separation of Concerns** - Specifications separate query logic from business logic
- **Reusable Queries** - Specifications can be reused across different handlers
- **Extension Methods** - Add EnsureExists() and other utility methods to specifications
- **Exception Pattern** - GetAsync() throws RecordNotFoundException, SingleOrDefault/FirstOrDefault return null
- **Type Safety** - Generic methods for mapping to different result types
