using Ardalis.Specification;
using FarmsManager.Application.Commands.Farms;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Specifications;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Application.Specifications.Feeds;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Queries.ProductionData;

public record GetRemainingFeedValueQueryResponse
{
    public decimal Value { get; init; }
}

public class GetRemainingFeedValueQuery : IRequest<BaseResponse<GetRemainingFeedValueQueryResponse>>
{
    public Guid CycleId { get; init; }
    public Guid HenhouseId { get; init; }
    public string FeedName { get; init; }
    public decimal Tonnage { get; init; }
}

public class
    GetRemainingFeedValueQueryHandler : IRequestHandler<GetRemainingFeedValueQuery,
    BaseResponse<GetRemainingFeedValueQueryResponse>>
{
    private readonly ICycleRepository _cycleRepository;
    private readonly IHenhouseRepository _henhouseRepository;
    private readonly IFeedNameRepository _feedNameRepository;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;

    public GetRemainingFeedValueQueryHandler(ICycleRepository cycleRepository, IHenhouseRepository henhouseRepository,
        IFeedNameRepository feedNameRepository, IFeedInvoiceRepository feedInvoiceRepository)
    {
        _cycleRepository = cycleRepository;
        _henhouseRepository = henhouseRepository;
        _feedNameRepository = feedNameRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
    }

    public async Task<BaseResponse<GetRemainingFeedValueQueryResponse>> Handle(GetRemainingFeedValueQuery request,
        CancellationToken ct)
    {
        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(request.CycleId), ct);
        var henhouse = await _henhouseRepository.GetAsync(new HenhouseByIdSpec(request.HenhouseId), ct);
        var feedName = await _feedNameRepository.GetAsync(new GetFeedNameByNameSpec(request.FeedName), ct);
        var feedInvoice =
            await _feedInvoiceRepository.FirstOrDefaultAsync(
                new GetFeedInvoiceByNameCycleAndHenhouseSpec(cycle.Id, henhouse.Id, feedName.Name), ct);


        var value = feedInvoice == null ? 0 : feedInvoice.UnitPrice * request.Tonnage;
        return BaseResponse.CreateResponse(new GetRemainingFeedValueQueryResponse
        {
            Value = value
        });
    }
}

public class GetRemainingFeedValueQueryValidator : AbstractValidator<GetRemainingFeedValueQuery>
{
    public GetRemainingFeedValueQueryValidator()
    {
        RuleFor(t => t.CycleId).NotEmpty();
        RuleFor(t => t.HenhouseId).NotEmpty();
        RuleFor(t => t.FeedName).NotEmpty();
        RuleFor(t => t.Tonnage).NotEmpty();
    }
}

public sealed class GetFeedInvoiceByNameCycleAndHenhouseSpec : BaseSpecification<FeedInvoiceEntity>
{
    public GetFeedInvoiceByNameCycleAndHenhouseSpec(Guid cycleId, Guid henhouseId, string feedName)
    {
        EnsureExists();
        DisableTracking();
        Query.Where(t => t.CycleId == cycleId);
        Query.Where(t => t.HenhouseId == henhouseId);
        Query.Where(t => t.ItemName == feedName);
        Query.OrderByDescending(t => t.InvoiceDate);
    }
}