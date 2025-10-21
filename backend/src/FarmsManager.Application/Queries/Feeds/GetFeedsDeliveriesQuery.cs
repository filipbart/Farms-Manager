using AutoMapper;
using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Extensions;
using FarmsManager.Application.Helpers;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models;
using FarmsManager.Application.Models.Notifications;
using FarmsManager.Application.Specifications.Feeds;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.Feeds;

public enum FeedsDeliveriesOrderType
{
    Priority,
    Cycle,
    Farm,
    HenhouseName,
    ItemName,
    VendorName,
    InvoiceNumber,
    Quantity,
    UnitPrice,
    InvoiceDate,
    DueDate,
    InvoiceTotal,
    SubTotal,
    VatAmount,
    PaymentDateUtc,
    DateCreatedUtc,
}

public record GetFeedsDeliveriesQueryFilters : OrderedPaginationParams<FeedsDeliveriesOrderType>
{
    public List<Guid> FarmIds { get; init; }
    public List<Guid> HenhouseIds { get; init; }
    public List<string> FeedNames { get; init; }
    public string InvoiceNumber { get; init; }
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
    public Guid CycleId { get; init; }
    public string CycleText { get; init; }
    public Guid FarmId { get; init; }
    public string FarmName { get; init; }
    public Guid HenhouseId { get; init; }
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
    public string CreatedByName { get; init; }
    public DateTime? DateModifiedUtc { get; init; }
    public string ModifiedByName { get; init; }
    public DateTime? DateDeletedUtc { get; init; }
    public string DeletedByName { get; init; }
    public NotificationPriority? Priority => PriorityCalculator.CalculatePriority(DueDate, PaymentDateUtc);
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
        var isAdmin = user.IsAdmin;

        var feedInvoices = await _feedInvoiceRepository.ListAsync<FeedDeliveryRowDto>(
            new GetAllFeedsDeliveriesSpec(request.Filters, accessibleFarmIds, isAdmin), ct);

        var feedCorrections = request.Filters.IncorrectPrices == true ||
                              (request.Filters.HenhouseIds != null && request.Filters.HenhouseIds.Count != 0)
            ? []
            : await _feedInvoiceCorrectionRepository.ListAsync<FeedDeliveryRowDto>(
                new GetAllFeedsCorrectionsSpec(request.Filters, accessibleFarmIds, isAdmin), ct);

        var combined = feedInvoices.Concat(feedCorrections).ClearAdminData(isAdmin);

        var orderedCombined = request.Filters.OrderBy switch
        {
            FeedsDeliveriesOrderType.Priority => combined
                .OrderByDescending(x => x.PaymentDateUtc == null && x.DueDate != null)
                .ThenBy(x =>
                {
                    if (x.PaymentDateUtc != null || x.DueDate == null) return 4;
                    var now = DateOnly.FromDateTime(DateTime.Now);
                    var daysUntilDue = x.DueDate.Value.DayNumber - now.DayNumber;
                    if (daysUntilDue < 0) return 1; // High - przeterminowane
                    if (daysUntilDue <= 7) return 2; // Medium - 0-7 dni
                    if (daysUntilDue <= 14) return 3; // Low - 8-14 dni
                    return 4; // Brak priorytetu - >14 dni
                })
                .ThenBy(x => x.DueDate),
            FeedsDeliveriesOrderType.Cycle => combined.SortBy(x => x.CycleText, request.Filters.IsDescending),
            FeedsDeliveriesOrderType.Farm => combined.SortBy(x => x.FarmName, request.Filters.IsDescending),
            FeedsDeliveriesOrderType.HenhouseName => combined.SortBy(x => x.HenhouseName, request.Filters.IsDescending),
            FeedsDeliveriesOrderType.ItemName => combined.SortBy(x => x.ItemName, request.Filters.IsDescending),
            FeedsDeliveriesOrderType.VendorName => combined.SortBy(x => x.VendorName, request.Filters.IsDescending),
            FeedsDeliveriesOrderType.InvoiceNumber => combined.SortBy(x => x.InvoiceNumber,
                request.Filters.IsDescending),
            FeedsDeliveriesOrderType.Quantity => combined.SortBy(x => x.Quantity, request.Filters.IsDescending),
            FeedsDeliveriesOrderType.UnitPrice => combined.SortBy(x => x.UnitPrice, request.Filters.IsDescending),
            FeedsDeliveriesOrderType.InvoiceDate => combined.SortBy(x => x.InvoiceDate, request.Filters.IsDescending),
            FeedsDeliveriesOrderType.DueDate => combined.SortBy(x => x.DueDate, request.Filters.IsDescending),
            FeedsDeliveriesOrderType.InvoiceTotal => combined.SortBy(x => x.InvoiceTotal, request.Filters.IsDescending),
            FeedsDeliveriesOrderType.SubTotal => combined.SortBy(x => x.SubTotal, request.Filters.IsDescending),
            FeedsDeliveriesOrderType.VatAmount => combined.SortBy(x => x.VatAmount, request.Filters.IsDescending),
            FeedsDeliveriesOrderType.PaymentDateUtc => combined.SortBy(x => x.PaymentDateUtc,
                request.Filters.IsDescending),
            FeedsDeliveriesOrderType.DateCreatedUtc => combined.SortBy(x => x.DateCreatedUtc,
                request.Filters.IsDescending),
            _ => combined.OrderByDescending(x => x.DateCreatedUtc)
        };

        var total = orderedCombined.Count();
        var skip = request.Filters.Page * request.Filters.PageSize;

        var pageItems = request.Filters.PageSize == -1
            ? orderedCombined.ToList()
            : orderedCombined.Skip(skip).Take(request.Filters.PageSize).ToList();

        return BaseResponse.CreateResponse(new GetFeedsDeliveriesQueryResponse
        {
            TotalRows = total,
            Items = pageItems.ToList()
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
            .ForMember(t => t.IsCorrection, opt => opt.MapFrom(t => false))
            .ForMember(t => t.CreatedByName, opt => opt.MapFrom(t => t.Creator != null ? t.Creator.Name : null))
            .ForMember(t => t.ModifiedByName, opt => opt.MapFrom(t => t.Modifier != null ? t.Modifier.Name : null))
            .ForMember(t => t.DeletedByName, opt => opt.MapFrom(t => t.Deleter != null ? t.Deleter.Name : null));

        CreateMap<FeedInvoiceCorrectionEntity, FeedDeliveryRowDto>()
            .ForMember(t => t.CycleText, opt => opt.MapFrom(t => t.Cycle.Identifier + "/" + t.Cycle.Year))
            .ForMember(t => t.FarmName, opt => opt.MapFrom(t => t.Farm.Name))
            .ForMember(t => t.IsCorrection, opt => opt.MapFrom(t => true))
            .ForMember(t => t.CreatedByName, opt => opt.MapFrom(t => t.Creator != null ? t.Creator.Name : null))
            .ForMember(t => t.ModifiedByName, opt => opt.MapFrom(t => t.Modifier != null ? t.Modifier.Name : null))
            .ForMember(t => t.DeletedByName, opt => opt.MapFrom(t => t.Deleter != null ? t.Deleter.Name : null));
    }
}

public static class LinqExtensions
{
    public static IOrderedEnumerable<TSource> SortBy<TSource, TKey>(this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector, bool isDescending)
    {
        return isDescending ? source.OrderByDescending(keySelector) : source.OrderBy(keySelector);
    }
}