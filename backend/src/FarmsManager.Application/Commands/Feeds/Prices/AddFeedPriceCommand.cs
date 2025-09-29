using FarmsManager.Application.Commands.Feeds.Names;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Application.Specifications.Feeds;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Feeds.Prices;

public record AddFeedPriceEntry
{
    public Guid NameId { get; init; }
    public decimal Price { get; init; }
}

public record AddFeedPriceCommand : IRequest<EmptyBaseResponse>
{
    public Guid FarmId { get; init; }
    public Guid IdentifierId { get; init; }
    public DateOnly PriceDate { get; init; }
    public List<AddFeedPriceEntry> Entries { get; init; }
}

public class AddFeedPriceCommandHandler : IRequestHandler<AddFeedPriceCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IFeedNameRepository _feedNameRepository;
    private readonly IFeedPriceRepository _feedPriceRepository;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;


    public AddFeedPriceCommandHandler(IUserDataResolver userDataResolver, IFarmRepository farmRepository,
        ICycleRepository cycleRepository, IFeedNameRepository feedNameRepository,
        IFeedPriceRepository feedPriceRepository, IFeedInvoiceRepository feedInvoiceRepository)
    {
        _userDataResolver = userDataResolver;
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
        _feedNameRepository = feedNameRepository;
        _feedPriceRepository = feedPriceRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
    }

    public async Task<EmptyBaseResponse> Handle(AddFeedPriceCommand request, CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.FarmId), ct);
        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(request.IdentifierId), ct);

        var newFeedPrices = new List<FeedPriceEntity>();
        var feedNamesToRecalculate = new HashSet<string>();

        foreach (var entry in request.Entries)
        {
            var feedNameEntity = await _feedNameRepository.GetAsync(new GetFeedNameByIdSpec(entry.NameId), ct);

            var newFeedPrice = FeedPriceEntity.CreateNew(
                farm.Id,
                cycle.Id,
                request.PriceDate,
                feedNameEntity.Name,
                entry.Price,
                userId);

            newFeedPrices.Add(newFeedPrice);
            feedNamesToRecalculate.Add(feedNameEntity.Name);
        }

        await _feedPriceRepository.AddRangeAsync(newFeedPrices, ct);

        foreach (var feedName in feedNamesToRecalculate)
        {
            var nextFeedPrice = await _feedPriceRepository.FirstOrDefaultAsync(
                new GetNextFeedPriceSpec(farm.Id, cycle.Id, feedName, request.PriceDate), ct);

            var startDate = request.PriceDate;
            var endDate = nextFeedPrice?.PriceDate ?? DateOnly.MaxValue;

            var feedsInvoices = await _feedInvoiceRepository.ListAsync(
                new GetFeedsInvoicesByDateRangeAndNameSpec(farm.Id, cycle.Id, startDate, endDate, feedName),
                ct);

            if (feedsInvoices.Count != 0)
            {
                foreach (var feedInvoiceEntity in feedsInvoices)
                {
                    var feedPrices = await _feedPriceRepository.GetFeedPricesForInvoiceDateAsync(
                        feedInvoiceEntity.FarmId, feedInvoiceEntity.CycleId,
                        feedInvoiceEntity.ItemName,
                        feedInvoiceEntity.InvoiceDate);

                    feedInvoiceEntity.CheckUnitPrice(feedPrices);
                    await _feedInvoiceRepository.UpdateAsync(feedInvoiceEntity, ct);
                }
            }
        }

        return new EmptyBaseResponse();
    }
}

public class AddFeedPriceCommandValidator : AbstractValidator<AddFeedPriceCommand>
{
    public AddFeedPriceCommandValidator()
    {
        RuleFor(t => t.FarmId).NotEmpty();
        RuleFor(t => t.IdentifierId).NotEmpty();
        RuleFor(t => t.PriceDate).NotEmpty();
        RuleFor(t => t.Entries).NotEmpty().WithMessage("Należy dodać co najmniej jedną pozycję.");

        RuleForEach(t => t.Entries).ChildRules(entry =>
        {
            entry.RuleFor(e => e.NameId).NotEmpty().WithMessage("Nazwa paszy jest wymagana.");
            entry.RuleFor(e => e.Price).GreaterThanOrEqualTo(0).WithMessage("Cena nie może być mniejsza niż 0.");
        });
    }
}