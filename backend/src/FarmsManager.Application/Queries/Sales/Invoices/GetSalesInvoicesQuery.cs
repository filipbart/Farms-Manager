using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models;
using MediatR;

namespace FarmsManager.Application.Queries.Sales.Invoices;

public enum SalesInvoicesOrderBy
{
    Priority,
    Cycle,
    Farm,
    Slaughterhouse,
    InvoiceNumber,
    InvoiceDate,
    DueDate,
    InvoiceTotal,
    SubTotal,
    VatAmount,
    DateCreatedUtc,
}

public record GetSalesInvoicesQueryFilters : OrderedPaginationParams<SalesInvoicesOrderBy>
{
    public List<Guid> FarmIds { get; init; }
    public List<Guid> SlaughterhouseIds { get; init; }
    public List<string> Cycles { get; init; }

    public List<CycleDictModel> CyclesDict => Cycles?.Select(c => new CycleDictModel
    {
        Identifier = int.Parse(c.Split('-')[0]),
        Year = int.Parse(c.Split('-')[1])
    }).ToList();

    public DateOnly? DateSince { get; init; }
    public DateOnly? DateTo { get; init; }
}

public record GetSalesInvoicesQuery(GetSalesInvoicesQueryFilters Filters)
    : IRequest<BaseResponse<GetSalesInvoicesQueryResponse>>;