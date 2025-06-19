using Ardalis.Specification;
using FarmsManager.Application.Common;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Application.Specifications;

public class BaseSpecification<T> : Specification<T> where T : Entity
{
    protected void DisableTracking()
    {
        Query.AsNoTracking();
    }

    protected void EnsureExists()
    {
        Query.Where(t => t.DateDeletedUtc.HasValue == false);
    }

    protected void Paginate(PaginationParams paginationParams)
    {
        Query.Skip(paginationParams.Skip).Take(paginationParams.PageSize);
    }
}

public class BaseSpecification<T, TResult> : Specification<T, TResult> where T : Entity
{
    protected void DisableTracking()
    {
        Query.AsNoTracking();
    }

    protected void EnsureExists()
    {
        Query.Where(t => t.DateDeletedUtc.HasValue == false);
    }

    protected void Paginate(PaginationParams paginationParams)
    {
        Query.Skip(paginationParams.Skip).Take(paginationParams.PageSize);
    }
}