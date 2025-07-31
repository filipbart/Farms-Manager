using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using MediatR;

namespace FarmsManager.Application.Queries.Gas.Deliveries;

public enum GasDeliveriesOrderBy
{
    Farm,
    Contractor,
    InvoiceDate,
    InvoiceNumber,
    UnitPrice,
    Quantity
}

public record GetGasDeliveriesQueryFilters : OrderedPaginationParams<GasDeliveriesOrderBy>
{
    public List<Guid> FarmIds { get; init; }
    public List<Guid> ContractorIds { get; init; }
    public DateOnly? DateSince { get; init; }
    public DateOnly? DateTo { get; init; }
}

public record GetGasDeliveriesQuery(GetGasDeliveriesQueryFilters Filters)
    : IRequest<BaseResponse<GetGasDeliveriesQueryResponse>>;