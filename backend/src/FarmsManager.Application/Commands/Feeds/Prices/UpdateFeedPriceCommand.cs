using Ardalis.Specification;
using FarmsManager.Application.Commands.Feeds.Names;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Application.Specifications.Feeds;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Feeds.Prices;

public record UpdateFeedPriceCommandDto
{
    public Guid CycleId { get; init; }
    public DateOnly PriceDate { get; init; }
    public Guid NameId { get; init; }
    public decimal Price { get; init; }
}

public record UpdateFeedPriceCommand(Guid Id, UpdateFeedPriceCommandDto Data) : IRequest<EmptyBaseResponse>;

public class UpdateFeedPriceCommandHandler : IRequestHandler<UpdateFeedPriceCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFeedNameRepository _feedNameRepository;
    private readonly IFeedPriceRepository _feedPriceRepository;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly ICycleRepository _cycleRepository;

    public UpdateFeedPriceCommandHandler(IUserDataResolver userDataResolver, IFeedNameRepository feedNameRepository,
        IFeedPriceRepository feedPriceRepository, IFeedInvoiceRepository feedInvoiceRepository,
        ICycleRepository cycleRepository)
    {
        _userDataResolver = userDataResolver;
        _feedNameRepository = feedNameRepository;
        _feedPriceRepository = feedPriceRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
        _cycleRepository = cycleRepository;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateFeedPriceCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var feedPrice = await _feedPriceRepository.GetAsync(new FeedPriceByIdSpec(request.Id), cancellationToken);
        var feedName =
            await _feedNameRepository.GetAsync(new GetFeedNameByIdSpec(request.Data.NameId), cancellationToken);
        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(request.Data.CycleId), cancellationToken);

        if (feedPrice.CycleId != cycle.Id)
        {
            feedPrice.SetCycle(cycle.Id);
        }

        feedPrice.Update(request.Data.PriceDate, feedName.Name, request.Data.Price);
        feedPrice.SetModified(userId);

        await _feedPriceRepository.UpdateAsync(feedPrice, cancellationToken);

        var nextFeedPrice = await _feedPriceRepository.FirstOrDefaultAsync(
            new GetNextFeedPriceWithoutCurrentSpec(feedName.Name, feedPrice.PriceDate, request.Id), cancellationToken);

        var startDate = feedPrice.PriceDate;

        var endDate = nextFeedPrice?.PriceDate ?? DateOnly.MaxValue;

        var feedsInvoices = await _feedInvoiceRepository.ListAsync(
            new GetFeedsInvoicesByDateRangeAndNameSpec(feedPrice.FarmId, feedPrice.CycleId, startDate, endDate,
                feedName.Name), cancellationToken);

        foreach (var feedInvoiceEntity in feedsInvoices)
        {
            var feedPrices =
                await _feedPriceRepository.ListAsync(
                    new GetFeedPriceForFeedInvoiceSpec(feedInvoiceEntity.FarmId, feedInvoiceEntity.CycleId,
                        feedInvoiceEntity.ItemName, feedInvoiceEntity.InvoiceDate), cancellationToken);

            feedInvoiceEntity.CheckUnitPrice(feedPrices);
            await _feedInvoiceRepository.UpdateAsync(feedInvoiceEntity, cancellationToken);
        }

        return new EmptyBaseResponse();
    }
}

public class UpdateFeedPriceCommandValidator : AbstractValidator<UpdateFeedPriceCommand>
{
    public UpdateFeedPriceCommandValidator()
    {
        RuleFor(t => t.Data.CycleId).NotEmpty();
        RuleFor(t => t.Data.Price).GreaterThanOrEqualTo(0).WithMessage("Cena nie moze być mniejsza niz 0");
    }
}

public sealed class GetNextFeedPriceWithoutCurrentSpec : BaseSpecification<FeedPriceEntity>,
    ISingleResultSpecification<FeedPriceEntity>
{
    public GetNextFeedPriceWithoutCurrentSpec(string feedName, DateOnly priceDate, Guid currentPriceId)
    {
        EnsureExists();
        Query.Where(t => t.Name == feedName)
            .Where(t => t.PriceDate >= priceDate)
            .Where(t => t.Id != currentPriceId)
            .OrderBy(t => t.PriceDate);
    }
}