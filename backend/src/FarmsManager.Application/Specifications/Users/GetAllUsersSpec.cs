using Ardalis.Specification;
using FarmsManager.Application.Queries.Users;
using FarmsManager.Domain.Aggregates.UserAggregate.Entities;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Application.Specifications.Users;

public sealed class GetAllUsersSpec : BaseSpecification<UserEntity>
{
    public GetAllUsersSpec(GetUsersQueryFilters filters, bool withPagination)
    {
        EnsureExists();
        DisableTracking();

        PopulateFilters(filters);
        ApplyOrdering(filters);

        if (withPagination)
        {
            Paginate(filters);
        }
    }

    private void PopulateFilters(GetUsersQueryFilters filters)
    {
        if (!string.IsNullOrWhiteSpace(filters.SearchPhrase))
        {
            var phrase = $"%{filters.SearchPhrase}%";
            Query.Where(u => EF.Functions.ILike(u.Name, phrase) || EF.Functions.ILike(u.Login, phrase));
        }
    }

    private void ApplyOrdering(GetUsersQueryFilters filters)
    {
        var isDescending = filters.IsDescending;
        switch (filters.OrderBy)
        {
            case UsersOrderBy.Name:
                if (isDescending)
                {
                    Query.OrderByDescending(u => u.Name);
                }
                else
                {
                    Query.OrderBy(u => u.Name);
                }
                break;

            case UsersOrderBy.Login:
                if (isDescending)
                {
                    Query.OrderByDescending(u => u.Login);
                }
                else
                {
                    Query.OrderBy(u => u.Login);
                }
                break;
            
            case UsersOrderBy.DateCreatedUtc:
            default:
                if (isDescending)
                {
                    Query.OrderByDescending(u => u.DateCreatedUtc);
                }
                else
                {
                    Query.OrderBy(u => u.DateCreatedUtc);
                }
                break;
        }
    }
}