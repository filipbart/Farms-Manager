using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;

namespace FarmsManager.Application.Specifications.Employees;

public sealed class GetEmployeeByIdWithFilesSpec : BaseSpecification<EmployeeEntity>,
    ISingleResultSpecification<EmployeeEntity>
{
    public GetEmployeeByIdWithFilesSpec(Guid employeeId)
    {
        EnsureExists();
        Query
            .Where(e => e.Id == employeeId)
            .Include(e => e.Files);
    }
}