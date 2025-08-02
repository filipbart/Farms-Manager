using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;

namespace FarmsManager.Application.Specifications.Employees;

public sealed class GetPayslipByIdSpec : BaseSpecification<EmployeePayslipEntity>,
    ISingleResultSpecification<EmployeePayslipEntity>
{
    public GetPayslipByIdSpec(Guid id)
    {
        EnsureExists();
        Query.Where(t => t.Id == id);
    }
}