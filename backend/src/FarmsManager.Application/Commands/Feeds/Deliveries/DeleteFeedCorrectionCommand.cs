using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Application.Specifications.Feeds;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Feeds.Deliveries;

public record DeleteFeedCorrectionCommand(Guid Id) : IRequest<EmptyBaseResponse>;

public class DeleteFeedCorrectionCommandHandler : IRequestHandler<DeleteFeedCorrectionCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFeedPriceRepository _feedPriceRepository;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly IFeedInvoiceCorrectionRepository _feedInvoiceCorrectionRepository;

    public DeleteFeedCorrectionCommandHandler(IUserDataResolver userDataResolver,
        IFeedPriceRepository feedPriceRepository, IFeedInvoiceRepository feedInvoiceRepository,
        IFeedInvoiceCorrectionRepository feedInvoiceCorrectionRepository)
    {
        _userDataResolver = userDataResolver;
        _feedPriceRepository = feedPriceRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
        _feedInvoiceCorrectionRepository = feedInvoiceCorrectionRepository;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteFeedCorrectionCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var correction = await _feedInvoiceCorrectionRepository.GetAsync(new GetFeedCorrectionByIdSpec(request.Id),
            cancellationToken);

        var feedInvoices =
            await _feedInvoiceRepository.ListAsync(new GetFeedInvoicesByCorrectionIdSpec(request.Id),
                cancellationToken);

        if (feedInvoices.Count != 0)
        {
            foreach (var feedInvoiceEntity in feedInvoices)
            {
                feedInvoiceEntity.SetInvoiceCorrectionId(null);
                feedInvoiceEntity.SetComment(null);
                await CheckFeedInvoiceUnitPrice(feedInvoiceEntity, cancellationToken);
            }

            await _feedInvoiceRepository.UpdateRangeAsync(feedInvoices, cancellationToken);
        }

        correction.Delete(userId);
        await _feedInvoiceCorrectionRepository.UpdateAsync(correction, cancellationToken);

        return new EmptyBaseResponse();
    }

    private async Task CheckFeedInvoiceUnitPrice(FeedInvoiceEntity feedInvoice, CancellationToken ct)
    {
        var feedPrices =
            await _feedPriceRepository.ListAsync(
                new GetFeedPriceForFeedInvoiceSpec(feedInvoice.FarmId, feedInvoice.ItemName, feedInvoice.InvoiceDate),
                ct);

        feedInvoice.CheckUnitPrice(feedPrices);
    }
}

public sealed class GetFeedCorrectionByIdSpec : BaseSpecification<FeedInvoiceCorrectionEntity>,
    ISingleResultSpecification<FeedInvoiceCorrectionEntity>
{
    public GetFeedCorrectionByIdSpec(Guid id)
    {
        EnsureExists();
        Query.Where(t => t.Id == id);
    }
}

public sealed class GetFeedInvoicesByCorrectionIdSpec : BaseSpecification<FeedInvoiceEntity>
{
    public GetFeedInvoicesByCorrectionIdSpec(Guid feedCorrectionId)
    {
        EnsureExists();
        Query.Where(t => t.InvoiceCorrectionId == feedCorrectionId);
    }
}