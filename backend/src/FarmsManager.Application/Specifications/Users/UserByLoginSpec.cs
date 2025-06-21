using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.UserAggregate.Entities;

namespace FarmsManager.Application.Specifications.Users;

public sealed class UserByLoginSpec : BaseSpecification<UserEntity>, ISingleResultSpecification<UserEntity>
{
    public UserByLoginSpec(string login)
    {
        EnsureExists();

        Query.Where(t => t.Login == login);
    }
}