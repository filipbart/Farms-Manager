using FarmsManager.Application.Common;
using FarmsManager.Application.Extensions;
using FarmsManager.Application.Models.Employees;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Enums;

namespace FarmsManager.Application.Queries.Employees;

public class EmployeeRowDto
{
    public Guid Id { get; init; }
    public string FarmName { get; init; }
    public string FullName { get; init; }
    public string Position { get; init; }
    public string ContractType { get; init; }
    public decimal Salary { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public EmployeeStatus Status { get; init; }
    public string StatusDesc => Status.GetDescription();
    public string Comment { get; init; }
    public List<EmployeeFileDto> Files { get; init; } = [];
    public bool UpcomingDeadline { get; init; }
    public DateTime DateCreatedUtc { get; init; }
    public string CreatedByName { get; init; }
    public DateTime? DateModifiedUtc { get; init; }
    public string ModifiedByName { get; init; }
    public DateTime? DateDeletedUtc { get; init; }
    public string DeletedByName { get; init; }
}

public class GetEmployeesQueryResponse : PaginationModel<EmployeeRowDto>;