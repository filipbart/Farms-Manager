using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entites;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Feeds.Deliveries;

public record DeleteFeedPaymentCommand(Guid Id) : IRequest<EmptyBaseResponse>;

public class DeleteFeedPaymentCommandHandler : IRequestHandler<DeleteFeedPaymentCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFeedPaymentRepository _feedPaymentRepository;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;

    public DeleteFeedPaymentCommandHandler(IUserDataResolver userDataResolver,
        IFeedPaymentRepository feedPaymentRepository, IFeedInvoiceRepository feedInvoiceRepository)
    {
        _userDataResolver = userDataResolver;
        _feedPaymentRepository = feedPaymentRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteFeedPaymentCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var feedPayment = await _feedPaymentRepository.GetAsync(new FeedPaymentByIdSpec(request.Id),
            cancellationToken);

        var feedInvoices =
            await _feedInvoiceRepository.ListAsync(new GetFeedInvoicesByPaymentIdSpec(request.Id), cancellationToken);

        if (feedInvoices.Count != 0)
        {
            foreach (var feedInvoiceEntity in feedInvoices)
            {
                feedInvoiceEntity.MarkAsUnpaid();
            }

            await _feedInvoiceRepository.UpdateRangeAsync(feedInvoices, cancellationToken);
        }

        feedPayment.Delete(userId);
        await _feedPaymentRepository.UpdateAsync(feedPayment, cancellationToken);

        return new EmptyBaseResponse();
    }
}

public sealed class FeedPaymentByIdSpec : BaseSpecification<FeedPaymentEntity>,
    ISingleResultSpecification<FeedPaymentEntity>
{
    public FeedPaymentByIdSpec(Guid id)
    {
        EnsureExists();
        Query.Where(t => t.Id == id);
    }
}

public sealed class GetFeedInvoicesByPaymentIdSpec : BaseSpecification<FeedInvoiceEntity>
{
    public GetFeedInvoicesByPaymentIdSpec(Guid feedPaymentId)
    {
        EnsureExists();
        Query.Where(t => t.PaymentId == feedPaymentId);
    }
}