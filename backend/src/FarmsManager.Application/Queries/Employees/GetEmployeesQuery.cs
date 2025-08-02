using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Enums;
using MediatR;

namespace FarmsManager.Application.Queries.Employees;

public enum EmployeesOrderBy
{
    FullName,
    Position,
    ContractType,
    Salary,
    StartDate,
    EndDate,
    Status
}

public record GetEmployeesQueryFilters : OrderedPaginationParams<EmployeesOrderBy>
{
    public List<Guid> FarmIds { get; init; }
    public string SearchPhrase { get; init; }
    public EmployeeStatus? Status { get; init; }
}

public record GetEmployeesQuery(GetEmployeesQueryFilters Filters)
    : IRequest<BaseResponse<GetEmployeesQueryResponse>>;