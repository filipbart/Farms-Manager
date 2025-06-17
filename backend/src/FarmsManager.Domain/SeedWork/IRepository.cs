using Ardalis.Specification;

namespace FarmsManager.Domain.SeedWork;

public interface IRepository;

public interface IRepository<T> : IRepositoryBase<T>, ICustomRepository<T> where T : Entity
{
    IUnitOfWork UnitOfWork { get; }
}

public interface ICustomRepository<T> : IRepository
{
    /// <summary>
    /// Zwraca listę
    /// </summary>
    /// <param name="spec"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    Task<List<TResult>> ListAsync<TResult>(ISpecification<T> spec, CancellationToken cancellationToken = default);

    /// <summary>
    /// Rzuca wyjątkiem 'RecordNotFoundException' jeśli nie znajdzie rekordu
    /// </summary>
    /// <param name="spec"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<T> GetAsync(ISingleResultSpecification<T> spec, CancellationToken cancellationToken = default);

    /// <summary>
    /// Rzuca wyjątkiem 'RecordNotFoundException' jeśli nie znajdzie rekordu i mapuje do wybranej postaci obiektu
    /// </summary>
    /// <param name="spec"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TResult> GetAsync<TResult>(ISingleResultSpecification<T> spec, CancellationToken cancellationToken = default);


    Task<TResult> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<T> spec,
        CancellationToken cancellationToken = default);

    Task<TResult> FirstOrDefaultAsync<TResult>(ISingleResultSpecification<T> spec,
        CancellationToken cancellationToken = default);

    void Update(T entity);
    void Add(T entity);
}