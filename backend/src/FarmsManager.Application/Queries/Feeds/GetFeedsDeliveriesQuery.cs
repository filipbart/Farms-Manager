using AutoMapper;
using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models;
using FarmsManager.Application.Specifications.Feeds;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
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
    public List<string> Cycles { get; init; }

    public List<CycleDictModel> CyclesDict => Cycles?.Select(c => new CycleDictModel
    {
        Identifier = int.Parse(c.Split('-')[0]),
        Year = int.Parse(c.Split('-')[1])
    }).ToList();

    public bool? IncorrectPrices { get; init; }
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
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;

    public GetFeedsDeliveriesQueryHandler(IFeedInvoiceRepository feedInvoiceRepository,
        IFeedInvoiceCorrectionRepository feedInvoiceCorrectionRepository, IUserDataResolver userDataResolver,
        IUserRepository userRepository)
    {
        _feedInvoiceRepository = feedInvoiceRepository;
        _feedInvoiceCorrectionRepository = feedInvoiceCorrectionRepository;
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<GetFeedsDeliveriesQueryResponse>> Handle(GetFeedsDeliveriesQuery request,
        CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), ct);
        var accessibleFarmIds = user.AccessibleFarmIds;

        var feedInvoices = await _feedInvoiceRepository.ListAsync<FeedDeliveryRowDto>(
            new GetAllFeedsDeliveriesSpec(request.Filters, false, accessibleFarmIds), ct);

        var feedCorrections = await _feedInvoiceCorrectionRepository.ListAsync<FeedDeliveryRowDto>(
            new GetAllFeedsCorrectionsSpec(request.Filters, false, accessibleFarmIds), ct);

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

        var pageItems = request.Filters.PageSize == -1
            ? combined
            : combined.Skip(skip).Take(request.Filters.PageSize).ToList();

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