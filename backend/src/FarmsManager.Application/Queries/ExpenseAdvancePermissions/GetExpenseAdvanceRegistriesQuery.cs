using Ardalis.Specification;
using AutoMapper;
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
    private readonly IMapper _mapper;

    public GetExpenseAdvancesListQueryHandler(IEmployeeRepository employeeRepository, IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _mapper = mapper;
    }

    public async Task<BaseResponse<GetExpenseAdvancesListQueryResponse>> Handle(
        GetExpenseAdvancesListQuery request,
        CancellationToken cancellationToken)
    {
        var employees = await _employeeRepository.ListAsync(
            new GetActiveEmployeesWithAdvancesSpec(),
            cancellationToken);

        var dtos = _mapper.Map<List<ExpenseAdvanceEntityDto>>(employees);

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
