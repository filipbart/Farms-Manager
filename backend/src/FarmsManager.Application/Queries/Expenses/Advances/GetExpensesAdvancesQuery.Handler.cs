using AutoMapper;
using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Extensions;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Services;
using FarmsManager.Application.Specifications.Employees;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Enums;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.Expenses.Advances;

public class GetExpensesAdvancesQueryHandler : IRequestHandler<GetExpensesAdvancesQuery,
    BaseResponse<GetExpensesAdvancesQueryResponse>>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IExpenseAdvanceRepository _expenseAdvanceRepository;
    private readonly IExpenseAdvancePermissionService _permissionService;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;

    public GetExpensesAdvancesQueryHandler(
        IEmployeeRepository employeeRepository,
        IExpenseAdvanceRepository expenseAdvanceRepository,
        IExpenseAdvancePermissionService permissionService,
        IUserDataResolver userDataResolver,
        IUserRepository userRepository)
    {
        _employeeRepository = employeeRepository;
        _expenseAdvanceRepository = expenseAdvanceRepository;
        _permissionService = permissionService;
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }


    public async Task<BaseResponse<GetExpensesAdvancesQueryResponse>> Handle(GetExpensesAdvancesQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken) 
            ?? throw DomainException.Unauthorized();
        
        // Dla admina nie filtrujemy - ma dostęp do wszystkich
        if (!user.IsAdmin)
        {
            // Sprawdź uprawnienia do przeglądania ewidencji tego pracownika
            var hasPermission = await _permissionService.HasPermissionAsync(
                userId, 
                request.EmployeeId, 
                ExpenseAdvancePermissionType.View,
                cancellationToken);

            if (!hasPermission)
                throw DomainException.Forbidden();
        }

        var employee = await _employeeRepository.GetAsync(new EmployeeByIdSpec(request.EmployeeId), cancellationToken);
        var isAdmin = user.IsAdmin;
        
        var data = await _expenseAdvanceRepository.ListAsync<ExpenseAdvanceRowDto>(
            new GetAllExpensesAdvancesSpec(request.EmployeeId, request.Filters, true, true, isAdmin), cancellationToken);
        var count = await _expenseAdvanceRepository.CountAsync(
            new GetAllExpensesAdvancesSpec(request.EmployeeId, request.Filters, false, true, isAdmin), cancellationToken);

        var allData = await _expenseAdvanceRepository.ListAsync(
            new GetAllExpensesAdvancesSpec(request.EmployeeId, request.Filters, false, false, isAdmin), cancellationToken);

        var allExpenses = allData.Where(t => t.Type == ExpenseAdvanceCategoryType.Expense).Sum(t => t.Amount);
        var allIncome = allData.Where(t => t.Type == ExpenseAdvanceCategoryType.Income).Sum(t => t.Amount);
        var allBalance = allIncome - allExpenses;

        var totalExpenses = data.Where(t => t.Type == ExpenseAdvanceCategoryType.Expense).Sum(t => t.Amount);
        var totalIncome = data.Where(t => t.Type == ExpenseAdvanceCategoryType.Income).Sum(t => t.Amount);
        var balance = totalIncome - totalExpenses;
        return BaseResponse.CreateResponse(new GetExpensesAdvancesQueryResponse
        {
            EmployeeFullName = employee.FullName,
            List = new PaginationModel<ExpenseAdvanceRowDto>
            {
                TotalRows = count,
                Items = data.ClearAdminData(isAdmin)
            },
            TotalBalance = allBalance,
            Balance = balance,
            TotalIncome = totalIncome,
            TotalExpenses = totalExpenses,
        });
    }
}

public class ExpenseAdvanceProfile : Profile
{
    public ExpenseAdvanceProfile()
    {
        CreateMap<ExpenseAdvanceEntity, ExpenseAdvanceRowDto>()
            .ForMember(m => m.CategoryName, opt => opt.MapFrom(t => t.ExpenseAdvanceCategory.Name));
    }
}