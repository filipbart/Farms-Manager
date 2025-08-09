using Ardalis.Specification;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Enums;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Application.Queries.Employees;

public sealed class GetAllEmployeesSpec : BaseSpecification<EmployeeEntity>
{
    public GetAllEmployeesSpec(GetEmployeesQueryFilters filters, bool withPagination)
    {
        EnsureExists();
        DisableTracking();

        Query.Include(e => e.Farm);
        Query.Include(e => e.Files);

        PopulateFilters(filters);
        ApplyOrdering(filters);

        if (withPagination)
        {
            Paginate(filters);
        }
    }

    private void PopulateFilters(GetEmployeesQueryFilters filters)
    {
        if (filters.FarmIds is not null && filters.FarmIds.Count != 0)
        {
            Query.Where(e => filters.FarmIds.Contains(e.FarmId));
        }

        if (!string.IsNullOrWhiteSpace(filters.SearchPhrase))
        {
            var phrase = $"%{filters.SearchPhrase}%";
            Query.Where(e => EF.Functions.ILike(e.FullName, phrase));
        }

        if (filters.Status is not null)
        {
            Query.Where(t => t.Status == filters.Status);
        }

        if (filters.AddToAdvances is not null)
        {
            Query.Where(t => t.AddToAdvances == filters.AddToAdvances);
        }
    }

    private void ApplyOrdering(GetEmployeesQueryFilters filters)
    {
        var isDescending = filters.IsDescending;
        switch (filters.OrderBy)
        {
            case EmployeesOrderBy.Position:
                if (isDescending)
                {
                    Query.OrderBy(e => e.Status != EmployeeStatus.Active).ThenByDescending(e => e.Position);
                }
                else
                {
                    Query.OrderBy(e => e.Status != EmployeeStatus.Active).ThenBy(e => e.Position);
                }

                break;

            case EmployeesOrderBy.ContractType:
                if (isDescending)
                {
                    Query.OrderBy(e => e.Status != EmployeeStatus.Active).ThenByDescending(e => e.ContractType);
                }
                else
                {
                    Query.OrderBy(e => e.Status != EmployeeStatus.Active).ThenBy(e => e.ContractType);
                }

                break;

            case EmployeesOrderBy.Salary:
                if (isDescending)
                {
                    Query.OrderBy(e => e.Status != EmployeeStatus.Active).ThenByDescending(e => e.Salary);
                }
                else
                {
                    Query.OrderBy(e => e.Status != EmployeeStatus.Active).ThenBy(e => e.Salary);
                }

                break;

            case EmployeesOrderBy.StartDate:
                if (isDescending)
                {
                    Query.OrderBy(e => e.Status != EmployeeStatus.Active).ThenByDescending(e => e.StartDate);
                }
                else
                {
                    Query.OrderBy(e => e.Status != EmployeeStatus.Active).ThenBy(e => e.StartDate);
                }

                break;

            case EmployeesOrderBy.EndDate:
                if (isDescending)
                {
                    Query.OrderBy(e => e.Status != EmployeeStatus.Active).ThenByDescending(e => e.EndDate);
                }
                else
                {
                    Query.OrderBy(e => e.Status != EmployeeStatus.Active).ThenBy(e => e.EndDate);
                }

                break;

            case EmployeesOrderBy.Status:
                if (isDescending)
                {
                    Query.OrderByDescending(e => e.Status);
                }
                else
                {
                    Query.OrderBy(e => e.Status);
                }

                break;

            case EmployeesOrderBy.FullName:
                if (isDescending)
                {
                    Query.OrderBy(e => e.Status != EmployeeStatus.Active).ThenByDescending(e => e.FullName);
                }
                else
                {
                    Query.OrderBy(e => e.Status != EmployeeStatus.Active).ThenBy(e => e.FullName);
                }

                break;
            default:
                if (isDescending)
                {
                    Query.OrderBy(e => e.Status != EmployeeStatus.Active)
                        .ThenByDescending(e => e.DateCreatedUtc);
                }
                else
                {
                    Query.OrderBy(e => e.Status != EmployeeStatus.Active)
                        .ThenBy(e => e.DateCreatedUtc);
                }

                break;
        }
    }
}