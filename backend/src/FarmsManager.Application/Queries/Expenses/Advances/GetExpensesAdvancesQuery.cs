using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using MediatR;

namespace FarmsManager.Application.Queries.Expenses.Advances;

public enum ExpensesAdvancesOrderBy
{
    Date,
    Type,
    Name,
    Amount,
    DateCreatedUtc
}

public record GetExpensesAdvancesQueryFilters : OrderedPaginationParams<ExpensesAdvancesOrderBy>
{
    public List<int> Years { get; init; }
    public List<int> Months { get; init; }
}

public record GetExpensesAdvancesQuery(Guid EmployeeId, GetExpensesAdvancesQueryFilters Filters)
    : IRequest<BaseResponse<GetExpensesAdvancesQueryResponse>>;