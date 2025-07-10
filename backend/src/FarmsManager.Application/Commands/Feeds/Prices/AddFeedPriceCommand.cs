using FarmsManager.Application.Commands.Feeds.Names;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entites;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Feeds.Prices;

public record AddFeedPriceCommand : IRequest<EmptyBaseResponse>
{
    public Guid FarmId { get; init; }
    public Guid IdentifierId { get; init; }
    public DateOnly PriceDate { get; init; }
    public Guid NameId { get; init; }
    public decimal Price { get; init; }
}

public class AddFeedPriceCommandHandler : IRequestHandler<AddFeedPriceCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IFeedNameRepository _feedNameRepository;
    private readonly IFeedPriceRepository _feedPriceRepository;


    public AddFeedPriceCommandHandler(IUserDataResolver userDataResolver, IFarmRepository farmRepository,
        ICycleRepository cycleRepository, IFeedNameRepository feedNameRepository,
        IFeedPriceRepository feedPriceRepository)
    {
        _userDataResolver = userDataResolver;
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
        _feedNameRepository = feedNameRepository;
        _feedPriceRepository = feedPriceRepository;
    }

    public async Task<EmptyBaseResponse> Handle(AddFeedPriceCommand request, CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.FarmId), ct);
        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(request.IdentifierId), ct);
        var feedNameEntity = await _feedNameRepository.GetAsync(new GetFeedNameByIdSpec(request.NameId), ct);

        var newFeedPrice =
            FeedPriceEntity.CreateNew(farm.Id, cycle.Id, request.PriceDate, feedNameEntity.Name, request.Price, userId);

        await _feedPriceRepository.AddAsync(newFeedPrice, ct);
        return new EmptyBaseResponse();
    }
}

public class AddFeedPriceCommandValidator : AbstractValidator<AddFeedPriceCommand>
{
    public AddFeedPriceCommandValidator()
    {
        RuleFor(t => t.Price).GreaterThanOrEqualTo(0).WithMessage("Cena nie może być mniejsza niż 0");
    }
}