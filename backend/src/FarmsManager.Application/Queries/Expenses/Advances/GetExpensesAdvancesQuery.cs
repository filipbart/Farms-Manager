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
    public DateOnly? DateSince { get; init; }
    public DateOnly? DateTo { get; init; }
}

public record GetExpensesAdvancesQuery(Guid EmployeeId, GetExpensesAdvancesQueryFilters Filters)
    : IRequest<BaseResponse<GetExpensesAdvancesQueryResponse>>;