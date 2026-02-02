using Ardalis.Specification;
using FarmsManager.Application.Commands.Accounting;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
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
    private readonly IFeedInvoiceCorrectionRepository _feedInvoiceCorrectionRepository;
    private readonly IKSeFInvoiceRepository _ksefInvoiceRepository;

    public DeleteFeedPaymentCommandHandler(IUserDataResolver userDataResolver,
        IFeedPaymentRepository feedPaymentRepository, IFeedInvoiceRepository feedInvoiceRepository,
        IFeedInvoiceCorrectionRepository feedInvoiceCorrectionRepository,
        IKSeFInvoiceRepository ksefInvoiceRepository)
    {
        _userDataResolver = userDataResolver;
        _feedPaymentRepository = feedPaymentRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
        _feedInvoiceCorrectionRepository = feedInvoiceCorrectionRepository;
        _ksefInvoiceRepository = ksefInvoiceRepository;
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
                
                // Synchronizacja statusu płatności do księgowości (KSeF)
                var ksefInvoice = await _ksefInvoiceRepository.FirstOrDefaultAsync(
                    new KSeFInvoiceByAssignedEntityIdSpec(feedInvoiceEntity.Id),
                    cancellationToken);

                if (ksefInvoice != null)
                {
                    ksefInvoice.Update(
                        paymentStatus: KSeFPaymentStatus.Unpaid,
                        paymentDate: null);
                    ksefInvoice.SetModified(userId);
                    await _ksefInvoiceRepository.UpdateAsync(ksefInvoice, cancellationToken);
                }
            }

            await _feedInvoiceRepository.UpdateRangeAsync(feedInvoices, cancellationToken);
        }

        var feedCorrections =
            await _feedInvoiceCorrectionRepository.ListAsync(new GetFeedCorrectionsByPaymentIdSpec(request.Id),
                cancellationToken);

        if (feedCorrections.Count != 0)
        {
            foreach (var feedCorrectionEntity in feedCorrections)
            {
                feedCorrectionEntity.MarkAsUnpaid();
            }

            await _feedInvoiceCorrectionRepository.UpdateRangeAsync(feedCorrections, cancellationToken);
        }

        feedPayment.Delete(userId);
        await _feedPaymentRepository.UpdateAsync(feedPayment, cancellationToken);

        return new EmptyBaseResponse();
    }
}

public sealed class GetFeedCorrectionsByPaymentIdSpec : BaseSpecification<FeedInvoiceCorrectionEntity>
{
    public GetFeedCorrectionsByPaymentIdSpec(Guid feedPaymentId)
    {
        EnsureExists();
        Query.Where(t => t.PaymentId == feedPaymentId);
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