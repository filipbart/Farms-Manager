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
    public decimal? Quantity { get; init; }
    public decimal? UnitPrice { get; init; }
    public DateOnly InvoiceDate { get; init; }
    public DateOnly? DueDate { get; init; }
    public decimal InvoiceTotal { get; init; }
    public decimal SubTotal { get; init; }
    public decimal VatAmount { get; init; }
    public string Comment { get; init; }
    public DateTime DateCreatedUtc { get; init; }
    public string InvoiceNumber { get; init; }
    public string BankAccountNumber { get; init; }
    public decimal? CorrectUnitPrice { get; init; }
    public DateTime? PaymentDateUtc { get; init; }
    public string FilePath { get; init; }
    public bool IsCorrection { get; init; }
}

public class
    GetFeedsDeliveriesQueryHandler : IRequestHandler<GetFeedsDeliveriesQuery,
    BaseResponse<GetFeedsDeliveriesQueryResponse>>
{
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly IFeedInvoiceCorrectionRepository _feedInvoiceCorrectionRepository;

    public GetFeedsDeliveriesQueryHandler(IFeedInvoiceRepository feedInvoiceRepository,
        IFeedInvoiceCorrectionRepository feedInvoiceCorrectionRepository)
    {
        _feedInvoiceRepository = feedInvoiceRepository;
        _feedInvoiceCorrectionRepository = feedInvoiceCorrectionRepository;
    }

    public async Task<BaseResponse<GetFeedsDeliveriesQueryResponse>> Handle(GetFeedsDeliveriesQuery request,
        CancellationToken ct)
    {
        var feedInvoices = await _feedInvoiceRepository.ListAsync<FeedDeliveryRowDto>(
            new GetAllFeedsDeliveriesSpec(request.Filters, false), ct);

        var feedCorrections = await _feedInvoiceCorrectionRepository.ListAsync<FeedDeliveryRowDto>(
            new GetAllFeedsCorrectionsSpec(request.Filters, false), ct);

        var combined = feedInvoices.Concat(feedCorrections).ToList();

        combined = request.Filters.OrderBy switch
        {
            FeedsDeliveriesOrderBy.DateCreatedUtc => request.Filters.IsDescending
                ? combined.OrderByDescending(x => x.DateCreatedUtc).ToList()
                : combined.OrderBy(x => x.DateCreatedUtc).ToList(),

            FeedsDeliveriesOrderBy.Cycle => request.Filters.IsDescending
                ? combined.OrderByDescending(x => x.CycleText).ToList()
                : combined.OrderBy(x => x.CycleText).ToList(),

            FeedsDeliveriesOrderBy.Farm => request.Filters.IsDescending
                ? combined.OrderByDescending(x => x.FarmName).ToList()
                : combined.OrderBy(x => x.FarmName).ToList(),

            FeedsDeliveriesOrderBy.ItemName => request.Filters.IsDescending
                ? combined.OrderByDescending(x => x.ItemName).ToList()
                : combined.OrderBy(x => x.ItemName).ToList(),

            FeedsDeliveriesOrderBy.VendorName => request.Filters.IsDescending
                ? combined.OrderByDescending(x => x.VendorName).ToList()
                : combined.OrderBy(x => x.VendorName).ToList(),

            FeedsDeliveriesOrderBy.UnitPrice => request.Filters.IsDescending
                ? combined.OrderByDescending(x => x.UnitPrice).ToList()
                : combined.OrderBy(x => x.UnitPrice).ToList(),

            _ => combined
        };

        var total = combined.Count;
        var skip = request.Filters.Page * request.Filters.PageSize;
        var pageItems = combined.Skip(skip).Take(request.Filters.PageSize).ToList();

        return BaseResponse.CreateResponse(new GetFeedsDeliveriesQueryResponse
        {
            TotalRows = total,
            Items = pageItems
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
            .ForMember(t => t.HenhouseName, opt => opt.MapFrom(t => t.Henhouse.Name))
            .ForMember(t => t.IsCorrection, opt => opt.MapFrom(t => false));

        CreateMap<FeedInvoiceCorrectionEntity, FeedDeliveryRowDto>()
            .ForMember(t => t.CycleText, opt => opt.MapFrom(t => t.Cycle.Identifier + "/" + t.Cycle.Year))
            .ForMember(t => t.FarmName, opt => opt.MapFrom(t => t.Farm.Name))
            .ForMember(t => t.IsCorrection, opt => opt.MapFrom(t => true));
    }
}