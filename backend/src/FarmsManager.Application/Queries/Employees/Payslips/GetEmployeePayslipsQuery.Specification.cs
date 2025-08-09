using Ardalis.Specification;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Application.Queries.Employees.Payslips;

public sealed class GetAllEmployeePayslipsSpec : BaseSpecification<EmployeePayslipEntity>
{
    public GetAllEmployeePayslipsSpec(GetEmployeePayslipsQueryFilters filters, bool withPagination)
    {
        EnsureExists();
        DisableTracking();

        Query.Include(p => p.Farm);
        Query.Include(p => p.Cycle);
        Query.Include(p => p.Employee);

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
            Query.Where(p => filters.Cycles.Contains(p.Cycle.Identifier + "/" + p.Cycle.Year));
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