using Ardalis.Specification;
using FarmsManager.Application.Commands.Feeds.Names;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Application.Specifications.Feeds;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Feeds.Prices;

public record UpdateFeedPriceCommandDto
{
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

    public UpdateFeedPriceCommandHandler(IUserDataResolver userDataResolver, IFeedNameRepository feedNameRepository,
        IFeedPriceRepository feedPriceRepository, IFeedInvoiceRepository feedInvoiceRepository)
    {
        _userDataResolver = userDataResolver;
        _feedNameRepository = feedNameRepository;
        _feedPriceRepository = feedPriceRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateFeedPriceCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var feedPrice = await _feedPriceRepository.GetAsync(new FeedPriceByIdSpec(request.Id), cancellationToken);
        var feedName =
            await _feedNameRepository.GetAsync(new GetFeedNameByIdSpec(request.Data.NameId), cancellationToken);

        feedPrice.Update(request.Data.PriceDate, feedName.Name, request.Data.Price);
        feedPrice.SetModified(userId);

        await _feedPriceRepository.UpdateAsync(feedPrice, cancellationToken);

        var feedPriceAfterDate = await _feedPriceRepository.FirstOrDefaultAsync(
            new GetFirstFeedPriceByNameAfterDateSpec(feedName.Name, request.Data.PriceDate), cancellationToken);

        if (feedPriceAfterDate != null)
        {
            var feedsInvoices = await _feedInvoiceRepository.ListAsync(
                new GetFeedsInvoicesByFeedNameAndDateSpec(feedName.Name, feedPrice.PriceDate,
                    feedPriceAfterDate.PriceDate),
                cancellationToken);

            foreach (var feedInvoiceEntity in feedsInvoices.Where(feedInvoiceEntity =>
                         feedInvoiceEntity.UnitPrice != feedPrice.Price))
            {
                feedInvoiceEntity.SetCorrectUnitPrice(feedPrice.Price);
                await _feedInvoiceRepository.UpdateAsync(feedInvoiceEntity, cancellationToken);
            }
        }

        return new EmptyBaseResponse();
    }
}

public class UpdateFeedPriceCommandValidator : AbstractValidator<UpdateFeedPriceCommand>
{
    public UpdateFeedPriceCommandValidator()
    {
        RuleFor(t => t.Data.Price).GreaterThanOrEqualTo(0).WithMessage("Cena nie moze być mniejsza niz 0");
    }
}

public sealed class GetFeedsInvoicesByFeedNameAndDateSpec : BaseSpecification<FeedInvoiceEntity>
{
    public GetFeedsInvoicesByFeedNameAndDateSpec(string feedName, DateOnly priceDate, DateOnly dateTo)
    {
        EnsureExists();

        Query.Where(t => t.ItemName == feedName);
        Query.Where(t => t.InvoiceDate >= priceDate);
        Query.Where(t => t.InvoiceDate < dateTo);
    }
}

public sealed class GetFirstFeedPriceByNameAfterDateSpec : BaseSpecification<FeedPriceEntity>,
    ISingleResultSpecification<FeedPriceEntity>
{
    public GetFirstFeedPriceByNameAfterDateSpec(string feedName, DateOnly priceDate)
    {
        EnsureExists();
        DisableTracking();

        Query.Where(t => t.Name == feedName);
        Query.Where(t => t.PriceDate >= priceDate);
        Query.OrderBy(t => t.PriceDate);
        Query.Take(1);
    }
}