using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FarmsManager.Domain.Exceptions;
using FarmsManager.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Infrastructure.Repositories;

public abstract class AbstractRepository<T> : RepositoryBase<T>, ICustomRepository<T> where T : Entity
{
    private readonly IConfigurationProvider _configurationProvider;
    private readonly DbContext _dbContext;


    protected AbstractRepository(DbContext dbContext, IConfigurationProvider configurationProvider) : base(dbContext)
    {
        _configurationProvider = configurationProvider;
        _dbContext = dbContext;
    }

    protected AbstractRepository(DbContext dbContext, ISpecificationEvaluator specificationEvaluator,
        IConfigurationProvider configurationProvider) : base(dbContext, specificationEvaluator)
    {
        _configurationProvider = configurationProvider;
        _dbContext = dbContext;
    }

    public async Task<List<TResult>> ListAsync<TResult>(ISpecification<T> spec,
        CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(spec).ProjectTo<TResult>(_configurationProvider);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<T> GetAsync(ISingleResultSpecification<T> spec, CancellationToken cancellationToken = default)
    {
        return await SingleOrDefaultAsync(spec, cancellationToken) ??
               throw DomainException.RecordNotFoundException(typeof(T).Name);
    }

    public async Task<TResult> GetAsync<TResult>(ISingleResultSpecification<T> spec,
        CancellationToken cancellationToken = default)
    {
        return await SingleOrDefaultAsync<TResult>(spec, cancellationToken) ??
               throw DomainException.RecordNotFoundException(typeof(T).Name);
    }

    public async Task<TResult> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<T> spec,
        CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(spec).ProjectTo<TResult>(_configurationProvider);
        return await query.SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<TResult> FirstOrDefaultAsync<TResult>(ISingleResultSpecification<T> spec,
        CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(spec).ProjectTo<TResult>(_configurationProvider);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public void Update(T entity)
    {
        _dbContext.Set<T>().Update(entity);
    }

    public void Add(T entity)
    {
        _dbContext.Set<T>().Add(entity);
    }
}