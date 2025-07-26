using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models;
using MediatR;

namespace FarmsManager.Application.Queries.Expenses.Productions;

public enum ExpensesProductionsOrderBy
{
    Cycle,
    Farm,
    Contractor,
    ExpenseType,
    InvoiceTotal,
    SubTotal,
    VatAmount,
    InvoiceDate,
    DateCreatedUtc,
}

public record GetExpensesProductionsFilters : OrderedPaginationParams<ExpensesProductionsOrderBy>
{
    public List<Guid> FarmIds { get; init; }
    public List<CycleDictModel> Cycles { get; init; }
    public List<Guid> ContractorIds { get; init; }
    public List<string> ExpensesTypeNames { get; init; }
    public DateOnly? DateSince { get; init; }
    public DateOnly? DateTo { get; init; }
}

public record GetExpensesProductionQuery(GetExpensesProductionsFilters Filters)
    : IRequest<BaseResponse<GetExpensesProductionQueryResponse>>;