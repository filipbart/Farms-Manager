using AutoMapper;
using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models;
using FarmsManager.Application.Specifications.Feeds;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entites;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Feeds;

public enum FeedsDeliveriesOrderBy
{
    DateCreatedUtc,
    Cycle,
    Farm,
    ItemName,
    VendorName,
    UnitPrice
}

public record GetFeedsDeliveriesQueryFilters : OrderedPaginationParams<FeedsDeliveriesOrderBy>
{
    public List<Guid> FarmIds { get; init; }
    public List<CycleDictModel> Cycles { get; init; }
    public DateOnly? DateSince { get; init; }
    public DateOnly? DateTo { get; init; }
}

public record GetFeedsDeliveriesQuery(GetFeedsDeliveriesQueryFilters Filters)
    : IRequest<BaseResponse<GetFeedsDeliveriesQueryResponse>>;

public class GetFeedsDeliveriesQueryResponse : PaginationModel<FeedDeliveryRowDto>;

public record FeedDeliveryRowDto
{
    public Guid Id { get; init; }
    public string CycleText { get; init; }
    public string FarmName { get; init; }
    public string HenhouseName { get; init; }
    public string VendorName { get; init; }
    public string ItemName { get; init; }
    public decimal Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public DateOnly InvoiceDate { get; init; }
    public DateOnly DueDate { get; init; }
    public decimal InvoiceTotal { get; init; }
    public decimal SubTotal { get; init; }
    public decimal VatAmount { get; init; }
    public string Comment { get; init; }
    public DateTime DateCreatedUtc { get; init; }
    public string InvoiceNumber { get; init; }
    public string BankAccountNumber { get; init; }
    public decimal? CorrectUnitPrice { get; init; }
    public DateTime? PaymentDateUtc { get; init; }
}

public class
    GetFeedsDeliveriesQueryHandler : IRequestHandler<GetFeedsDeliveriesQuery,
    BaseResponse<GetFeedsDeliveriesQueryResponse>>
{
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;

    public GetFeedsDeliveriesQueryHandler(IFeedInvoiceRepository feedInvoiceRepository)
    {
        _feedInvoiceRepository = feedInvoiceRepository;
    }

    public async Task<BaseResponse<GetFeedsDeliveriesQueryResponse>> Handle(GetFeedsDeliveriesQuery request,
        CancellationToken ct)
    {
        var data = await _feedInvoiceRepository.ListAsync<FeedDeliveryRowDto>(
            new GetAllFeedsDeliveriesSpec(request.Filters, true), ct);
        var itemRows =
            await _feedInvoiceRepository.CountAsync(new GetAllFeedsDeliveriesSpec(request.Filters, false), ct);

        return BaseResponse.CreateResponse(new GetFeedsDeliveriesQueryResponse
        {
            TotalRows = itemRows,
            Items = data
        });
    }
}

public class FeedsDeliveriesProfile : Profile
{
    public FeedsDeliveriesProfile()
    {
        CreateMap<FeedInvoiceEntity, FeedDeliveryRowDto>()
            .ForMember(t => t.CycleText, opt => opt.MapFrom(t => t.Cycle.Identifier + "/" + t.Cycle.Year))
            .ForMember(t => t.FarmName, opt => opt.MapFrom(t => t.Farm.Name))
            .ForMember(t => t.HenhouseName, opt => opt.MapFrom(t => t.Henhouse.Name));
    }
}