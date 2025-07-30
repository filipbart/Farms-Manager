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

namespace FarmsManager.Application.Commands.ProductionData.TransferFeed;

public record AddProductionDataTransferFeedCommand : IRequest<EmptyBaseResponse>
{
    public Guid FromFarmId { get; init; }
    public Guid FromHenhouseId { get; init; }
    public Guid FromCycleId { get; init; }
    public Guid ToFarmId { get; init; }
    public Guid ToHenhouseId { get; init; }
    public Guid ToCycleId { get; init; }
    public string FeedName { get; init; }
    public decimal Tonnage { get; init; }
    public decimal Value { get; init; }
}

public class
    AddProductionDataTransferFeedCommandHandler : IRequestHandler<AddProductionDataTransferFeedCommand,
    EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IHenhouseRepository _henhouseRepository;
    private readonly IFeedNameRepository _feedNameRepository;
    private readonly IProductionDataTransferFeedRepository _transferFeedRepository;

    public AddProductionDataTransferFeedCommandHandler(
        IUserDataResolver userDataResolver,
        IFarmRepository farmRepository,
        ICycleRepository cycleRepository,
        IHenhouseRepository henhouseRepository,
        IProductionDataTransferFeedRepository transferFeedRepository,
        IFeedNameRepository feedNameRepository)
    {
        _userDataResolver = userDataResolver;
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
        _henhouseRepository = henhouseRepository;
        _transferFeedRepository = transferFeedRepository;
        _feedNameRepository = feedNameRepository;
    }

    public async Task<EmptyBaseResponse> Handle(AddProductionDataTransferFeedCommand request, CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var fromFarm = await _farmRepository.GetAsync(new FarmByIdSpec(request.FromFarmId), ct);
        var fromCycle = await _cycleRepository.GetAsync(new CycleByIdSpec(request.FromCycleId), ct);
        var fromHenhouse = await _henhouseRepository.GetAsync(new HenhouseByIdSpec(request.FromHenhouseId), ct);

        var toFarm = await _farmRepository.GetAsync(new FarmByIdSpec(request.ToFarmId), ct);
        var toCycle = await _cycleRepository.GetAsync(new CycleByIdSpec(request.ToCycleId), ct);
        var toHenhouse = await _henhouseRepository.GetAsync(new HenhouseByIdSpec(request.ToHenhouseId), ct);

        var feedName = await _feedNameRepository.GetAsync(new GetFeedNameByNameSpec(request.FeedName), ct);

        var newTransfer = ProductionDataTransferFeedEntity.CreateNew(
            fromFarm.Id,
            fromCycle.Id,
            fromHenhouse.Id,
            toFarm.Id,
            toCycle.Id,
            toHenhouse.Id,
            feedName.Name,
            request.Tonnage,
            request.Value,
            userId
        );

        await _transferFeedRepository.AddAsync(newTransfer, ct);

        return BaseResponse.EmptyResponse;
    }
}

public class AddProductionDataTransferFeedValidator : AbstractValidator<AddProductionDataTransferFeedCommand>
{
    public AddProductionDataTransferFeedValidator()
    {
        RuleFor(t => t.FromFarmId).NotEmpty();
        RuleFor(t => t.FromHenhouseId).NotEmpty();
        RuleFor(t => t.FromCycleId).NotEmpty();
        RuleFor(t => t.ToFarmId).NotEmpty();
        RuleFor(t => t.ToHenhouseId).NotEmpty();
        RuleFor(t => t.ToCycleId).NotEmpty();
        RuleFor(t => t.FeedName).NotEmpty();
        RuleFor(t => t.Tonnage).GreaterThan(0);
        RuleFor(t => t.Value).GreaterThanOrEqualTo(0);
        RuleFor(t => t.FromHenhouseId).NotEqual(t => t.ToHenhouseId);
    }
}