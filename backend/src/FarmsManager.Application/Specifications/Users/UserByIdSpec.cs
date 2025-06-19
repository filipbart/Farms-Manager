using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.UserAggregate.Entites;

namespace FarmsManager.Application.Specifications.Users;

public sealed class UserByIdSpec : BaseSpecification<UserEntity>, ISingleResultSpecification<UserEntity>
{
    public UserByIdSpec(Guid id)
    {
        EnsureExists();

        Query.Where(t => t.Id == id);
    }
}