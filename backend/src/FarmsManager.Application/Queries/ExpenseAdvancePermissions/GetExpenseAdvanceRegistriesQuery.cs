using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models.ExpenseAdvancePermissions;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Enums;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.ExpenseAdvancePermissions;

public record GetExpenseAdvancesListQuery : IRequest<BaseResponse<GetExpenseAdvancesListQueryResponse>>;

public record GetExpenseAdvancesListQueryResponse
{
    public List<ExpenseAdvanceEntityDto> ExpenseAdvances { get; init; }
}

public class GetExpenseAdvancesListQueryHandler : IRequestHandler<GetExpenseAdvancesListQuery,
    BaseResponse<GetExpenseAdvancesListQueryResponse>>
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetExpenseAdvancesListQueryHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<BaseResponse<GetExpenseAdvancesListQueryResponse>> Handle(
        GetExpenseAdvancesListQuery request,
        CancellationToken cancellationToken)
    {
        var employees = await _employeeRepository.ListAsync(
            new GetActiveEmployeesWithAdvancesSpec(),
            cancellationToken);

        var dtos = employees.Select(e => new ExpenseAdvanceEntityDto
        {
            Id = e.Id,
            EmployeeId = e.Id.ToString(),
            EmployeeName = e.FullName,
            Description = e.Position
        }).ToList();

        return BaseResponse.CreateResponse(new GetExpenseAdvancesListQueryResponse
        {
            ExpenseAdvances = dtos
        });
    }
}

public sealed class GetActiveEmployeesWithAdvancesSpec : BaseSpecification<EmployeeEntity>
{
    public GetActiveEmployeesWithAdvancesSpec()
    {
        EnsureExists();
        Query.Where(e => e.Status == EmployeeStatus.Active && e.AddToAdvances);
        Query.OrderBy(e => e.FullName);
    }
}
