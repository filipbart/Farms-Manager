using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;

namespace FarmsManager.Application.Specifications.Employees;

public sealed class EmployeeByIdSpec : BaseSpecification<EmployeeEntity>, ISingleResultSpecification<EmployeeEntity>
{
    public EmployeeByIdSpec(Guid id)
    {
        EnsureExists();
        Query.Where(t => t.Id == id);
    }
}