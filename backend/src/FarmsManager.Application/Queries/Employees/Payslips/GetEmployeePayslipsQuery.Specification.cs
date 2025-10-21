using Ardalis.Specification;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Application.Queries.Employees.Payslips;

public sealed class GetAllEmployeePayslipsSpec : BaseSpecification<EmployeePayslipEntity>
{
    public GetAllEmployeePayslipsSpec(GetEmployeePayslipsQueryFilters filters, bool withPagination,
        List<Guid> accessibleFarmIds, bool isAdmin)
    {
        EnsureExists(filters.ShowDeleted, isAdmin);
        DisableTracking();

        Query.Include(p => p.Farm);
        Query.Include(p => p.Cycle);
        Query.Include(p => p.Employee);
        Query.Include(p => p.Creator);
        Query.Include(p => p.Modifier);
        Query.Include(p => p.Deleter);

        if (accessibleFarmIds is not null && accessibleFarmIds.Count != 0)
            Query.Where(p => accessibleFarmIds.Contains(p.FarmId));

        PopulateFilters(filters);
        ApplyOrdering(filters);

        if (withPagination)
        {
            Paginate(filters);
        }
    }

    private void PopulateFilters(GetEmployeePayslipsQueryFilters filters)
    {
        if (filters.FarmIds is not null && filters.FarmIds.Count != 0)
        {
            Query.Where(p => filters.FarmIds.Contains(p.FarmId));
        }

        if (filters.CyclesDict is not null && filters.CyclesDict.Count != 0)
        {
            var predicate = PredicateBuilder.New<EmployeePayslipEntity>();

            predicate = filters.CyclesDict.Aggregate(predicate,
                (current, cycleFilter) => current.Or(t =>
                    t.Cycle.Identifier == cycleFilter.Identifier && t.Cycle.Year == cycleFilter.Year));

            Query.Where(predicate);
        }

        if (!string.IsNullOrWhiteSpace(filters.SearchPhrase))
        {
            var phrase = $"%{filters.SearchPhrase}%";
            Query.Where(p => EF.Functions.ILike(p.Employee.FullName, phrase));
        }

        if (filters.PayrollPeriod is not null)
        {
            Query.Where(p => p.PayrollPeriod == filters.PayrollPeriod);
        }
    }

    private void ApplyOrdering(GetEmployeePayslipsQueryFilters filters)
    {
        var isDescending = filters.IsDescending;
        switch (filters.OrderBy)
        {
            case EmployeePayslipsOrderBy.FarmName:
                if (isDescending)
                {
                    Query.OrderByDescending(p => p.Farm.Name);
                }
                else
                {
                    Query.OrderBy(p => p.Farm.Name);
                }

                break;

            case EmployeePayslipsOrderBy.Cycle:
                if (isDescending)
                {
                    Query.OrderByDescending(p => p.Cycle.Year).ThenByDescending(p => p.Cycle.Identifier);
                }
                else
                {
                    Query.OrderBy(p => p.Cycle.Year).ThenBy(p => p.Cycle.Identifier);
                }

                break;

            case EmployeePayslipsOrderBy.EmployeeFullName:
                if (isDescending)
                {
                    Query.OrderByDescending(p => p.Employee.FullName);
                }
                else
                {
                    Query.OrderBy(p => p.Employee.FullName);
                }

                break;

            case EmployeePayslipsOrderBy.PayrollPeriod:
                if (isDescending)
                {
                    Query.OrderByDescending(p => p.PayrollPeriod);
                }
                else
                {
                    Query.OrderBy(p => p.PayrollPeriod);
                }

                break;

            case EmployeePayslipsOrderBy.NetPay:
                if (isDescending)
                {
                    Query.OrderByDescending(p => p.NetPay);
                }
                else
                {
                    Query.OrderBy(p => p.NetPay);
                }

                break;

            case EmployeePayslipsOrderBy.BaseSalary:
                if (isDescending)
                {
                    Query.OrderByDescending(p => p.BaseSalary);
                }
                else
                {
                    Query.OrderBy(p => p.BaseSalary);
                }

                break;

            case EmployeePayslipsOrderBy.BonusAmount:
                if (isDescending)
                {
                    Query.OrderByDescending(p => p.BonusAmount);
                }
                else
                {
                    Query.OrderBy(p => p.BonusAmount);
                }

                break;

            default:
                if (isDescending)
                {
                    Query.OrderByDescending(p => p.DateCreatedUtc);
                }
                else
                {
                    Query.OrderBy(p => p.DateCreatedUtc);
                }

                break;
        }
    }
}