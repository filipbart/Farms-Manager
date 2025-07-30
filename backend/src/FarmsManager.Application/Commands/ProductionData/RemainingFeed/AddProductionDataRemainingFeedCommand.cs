using FarmsManager.Application.Commands.Farms;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Application.Specifications.Feeds;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.ProductionData.RemainingFeed;

public record AddProductionDataRemainingFeedCommand : IRequest<EmptyBaseResponse>
{
    public Guid FarmId { get; init; }
    public Guid HenhouseId { get; init; }
    public Guid CycleId { get; init; }
    public string FeedName { get; init; }
    public decimal RemainingTonnage { get; init; }
    public decimal RemainingValue { get; init; }
}

public class
    AddProductionDataRemainingFeedCommandHandler : IRequestHandler<AddProductionDataRemainingFeedCommand,
    EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IHenhouseRepository _henhouseRepository;
    private readonly IFeedNameRepository _feedNameRepository;
    private readonly IProductionDataRemainingFeedRepository _productionDataRemainingFeedRepository;

    public AddProductionDataRemainingFeedCommandHandler(
        IUserDataResolver userDataResolver,
        IFarmRepository farmRepository,
        ICycleRepository cycleRepository,
        IHenhouseRepository henhouseRepository,
        IProductionDataRemainingFeedRepository productionDataRemainingFeedRepository,
        IFeedNameRepository feedNameRepository)
    {
        _userDataResolver = userDataResolver;
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
        _henhouseRepository = henhouseRepository;
        _productionDataRemainingFeedRepository = productionDataRemainingFeedRepository;
        _feedNameRepository = feedNameRepository;
    }

    public async Task<EmptyBaseResponse> Handle(AddProductionDataRemainingFeedCommand request, CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.FarmId), ct);
        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(request.CycleId), ct);
        var henhouse = await _henhouseRepository.GetAsync(new HenhouseByIdSpec(request.HenhouseId), ct);
        var feedName = await _feedNameRepository.GetAsync(new GetFeedNameByNameSpec(request.FeedName), ct);

        var newRemainingFeed = ProductionDataRemainingFeedEntity.CreateNew(
            farm.Id,
            cycle.Id,
            henhouse.Id,
            feedName.Name,
            request.RemainingTonnage,
            request.RemainingValue,
            userId
        );

        await _productionDataRemainingFeedRepository.AddAsync(newRemainingFeed, ct);

        return BaseResponse.EmptyResponse;
    }
}

public class AddProductionDataRemainingFeedValidator : AbstractValidator<AddProductionDataRemainingFeedCommand>
{
    public AddProductionDataRemainingFeedValidator()
    {
        RuleFor(t => t.FarmId).NotEmpty();
        RuleFor(t => t.CycleId).NotEmpty();
        RuleFor(t => t.HenhouseId).NotEmpty();
        RuleFor(t => t.FeedName).NotEmpty();
        RuleFor(t => t.RemainingTonnage).GreaterThanOrEqualTo(0);
        RuleFor(t => t.RemainingValue).GreaterThanOrEqualTo(0);
    }
}