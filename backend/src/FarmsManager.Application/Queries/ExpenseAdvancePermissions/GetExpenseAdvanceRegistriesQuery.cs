using Ardalis.Specification;
using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.ExpenseAdvancePermissions;
using FarmsManager.Application.Services;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Enums;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
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
    private readonly IExpenseAdvancePermissionService _permissionService;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IMapper _mapper;

    public GetExpenseAdvancesListQueryHandler(
        IEmployeeRepository employeeRepository,
        IExpenseAdvancePermissionService permissionService,
        IUserDataResolver userDataResolver,
        IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _permissionService = permissionService;
        _userDataResolver = userDataResolver;
        _mapper = mapper;
    }

    public async Task<BaseResponse<GetExpenseAdvancesListQueryResponse>> Handle(
        GetExpenseAdvancesListQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        // Pobierz wszystkich aktywnych pracowników z zaliczkami
        var employees = await _employeeRepository.ListAsync(
            new GetActiveEmployeesWithAdvancesSpec(),
            cancellationToken);

        // Filtruj według uprawnień (jeśli nie admin)
        var accessibleEmployeeIds = await _permissionService.GetAccessibleEmployeeIdsAsync(userId, cancellationToken);
        
        // null oznacza admin - ma dostęp do wszystkich
        // pusta lista oznacza brak dostępu do nikogo
        if (accessibleEmployeeIds != null)
        {
            employees = employees.Where(e => accessibleEmployeeIds.Contains(e.Id)).ToList();
        }

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
