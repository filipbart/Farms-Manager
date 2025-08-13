using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Feeds;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Feeds.Prices;

public record DeleteFeedPriceCommand(Guid Id) : IRequest<EmptyBaseResponse>;

public class DeleteFeedPriceCommandHandler : IRequestHandler<DeleteFeedPriceCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFeedPriceRepository _feedPriceRepository;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;

    public DeleteFeedPriceCommandHandler(IUserDataResolver userDataResolver, IFeedPriceRepository feedPriceRepository,
        IFeedInvoiceRepository feedInvoiceRepository)
    {
        _userDataResolver = userDataResolver;
        _feedPriceRepository = feedPriceRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteFeedPriceCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var feedPriceToDelete = await _feedPriceRepository.GetAsync(new FeedPriceByIdSpec(request.Id),
            cancellationToken);

        if (feedPriceToDelete is null)
        {
            return new EmptyBaseResponse();
        }

        var nextFeedPrice = await _feedPriceRepository.FirstOrDefaultAsync(
            new GetNextFeedPriceSpec(feedPriceToDelete.FarmId, feedPriceToDelete.CycleId, feedPriceToDelete.Name,
                feedPriceToDelete.PriceDate), cancellationToken);

        var startDate = feedPriceToDelete.PriceDate;
        var endDate = nextFeedPrice?.PriceDate ?? DateOnly.MaxValue;

        var feedsInvoicesToUpdate = await _feedInvoiceRepository.ListAsync(
            new GetFeedsInvoicesByDateRangeAndNameSpec(feedPriceToDelete.FarmId, feedPriceToDelete.CycleId,
                startDate, endDate, feedPriceToDelete.Name),
            cancellationToken);

        foreach (var invoice in feedsInvoicesToUpdate)
        {
            invoice.SetAsNullCorrectUnitPrice();
            await _feedInvoiceRepository.UpdateAsync(invoice, cancellationToken);
        }

        feedPriceToDelete.Delete(userId);
        await _feedPriceRepository.UpdateAsync(feedPriceToDelete, cancellationToken);

        return new EmptyBaseResponse();
    }
}