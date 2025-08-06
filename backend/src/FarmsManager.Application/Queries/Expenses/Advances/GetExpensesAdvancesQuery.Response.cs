using FarmsManager.Application.Common;
using FarmsManager.Application.Extensions;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Enums;

namespace FarmsManager.Application.Queries.Expenses.Advances;

public class ExpenseAdvanceRowDto
{
    public Guid Id { get; init; }
    public DateOnly Date { get; init; }
    public ExpenseAdvanceCategoryType Type { get; init; }
    public string TypeDesc => Type.GetDescription();
    public string Name { get; init; }
    public decimal Amount { get; init; }
    public string CategoryName { get; init; }
    public string Comment { get; init; }
    public string FilePath { get; init; }
    public DateTime DateCreatedUtc { get; init; }
}

public class GetExpensesAdvancesQueryResponse
{
    public string EmployeeFullName { get; init; }
    public PaginationModel<ExpenseAdvanceRowDto> List { get; init; }
    public decimal Balance { get; init; }
    public decimal TotalIncome { get; init; }
    public decimal TotalExpenses { get; init; }
} 