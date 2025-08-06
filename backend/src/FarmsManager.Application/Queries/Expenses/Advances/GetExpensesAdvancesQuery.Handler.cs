using AutoMapper;
using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Specifications.Employees;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Enums;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Expenses.Advances;

public class GetExpensesAdvancesQueryHandler : IRequestHandler<GetExpensesAdvancesQuery,
    BaseResponse<GetExpensesAdvancesQueryResponse>>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IExpenseAdvanceRepository _expenseAdvanceRepository;

    public GetExpensesAdvancesQueryHandler(IEmployeeRepository employeeRepository,
        IExpenseAdvanceRepository expenseAdvanceRepository)
    {
        _employeeRepository = employeeRepository;
        _expenseAdvanceRepository = expenseAdvanceRepository;
    }


    public async Task<BaseResponse<GetExpensesAdvancesQueryResponse>> Handle(GetExpensesAdvancesQuery request,
        CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetAsync(new EmployeeByIdSpec(request.EmployeeId), cancellationToken);
        var data = await _expenseAdvanceRepository.ListAsync<ExpenseAdvanceRowDto>(
            new GetAllExpensesAdvancesSpec(request.EmployeeId, request.Filters, true), cancellationToken);
        var count = await _expenseAdvanceRepository.CountAsync(
            new GetAllExpensesAdvancesSpec(request.EmployeeId, request.Filters, false), cancellationToken);

        var totalExpenses = data.Where(t => t.Type == ExpenseAdvanceCategoryType.Expense).Sum(t => t.Amount);
        var totalIncome = data.Where(t => t.Type == ExpenseAdvanceCategoryType.Income).Sum(t => t.Amount);
        var balance = totalIncome - totalExpenses;
        return BaseResponse.CreateResponse(new GetExpensesAdvancesQueryResponse
        {
            EmployeeFullName = employee.FullName,
            List = new PaginationModel<ExpenseAdvanceRowDto>
            {
                TotalRows = count,
                Items = data
            },
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