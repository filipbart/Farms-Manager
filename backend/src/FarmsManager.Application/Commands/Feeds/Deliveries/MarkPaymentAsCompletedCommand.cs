using FarmsManager.Application.Commands.Accounting;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Feeds;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Feeds.Deliveries;

public record MarkPaymentAsCompletedCommandDto
{
    public string Comment { get; init; }
}

public record MarkPaymentAsCompletedCommand(Guid Id, MarkPaymentAsCompletedCommandDto Data) 
    : IRequest<EmptyBaseResponse>;

public class MarkPaymentAsCompletedCommandHandler 
    : IRequestHandler<MarkPaymentAsCompletedCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFeedPaymentRepository _feedPaymentRepository;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly IKSeFInvoiceRepository _ksefInvoiceRepository;

    public MarkPaymentAsCompletedCommandHandler(
        IUserDataResolver userDataResolver,
        IFeedPaymentRepository feedPaymentRepository,
        IFeedInvoiceRepository feedInvoiceRepository,
        IKSeFInvoiceRepository ksefInvoiceRepository)
    {
        _userDataResolver = userDataResolver;
        _feedPaymentRepository = feedPaymentRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
        _ksefInvoiceRepository = ksefInvoiceRepository;
    }

    public async Task<EmptyBaseResponse> Handle(
        MarkPaymentAsCompletedCommand request, 
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        
        var feedPayment = await _feedPaymentRepository.GetAsync(
            new FeedPaymentByIdSpec(request.Id), 
            cancellationToken);

        feedPayment.MarkAsCompleted(request.Data.Comment);
        
        await _feedPaymentRepository.UpdateAsync(feedPayment, cancellationToken);

        // Synchronizacja statusu płatności do księgowości (KSeF)
        // Znajdź wszystkie faktury pasz powiązane z tym przelewem i zaktualizuj status w księgowości
        var feedInvoices = await _feedInvoiceRepository.ListAsync(
            new FeedInvoicesByPaymentIdSpec(request.Id), 
            cancellationToken);

        foreach (var feedInvoice in feedInvoices)
        {
            // Znajdź fakturę KSeF powiązaną z tą fakturą paszy
            var ksefInvoice = await _ksefInvoiceRepository.FirstOrDefaultAsync(
                new KSeFInvoiceByAssignedEntityIdSpec(feedInvoice.Id),
                cancellationToken);

            if (ksefInvoice != null)
            {
                ksefInvoice.Update(paymentStatus: KSeFPaymentStatus.PaidTransfer);
                ksefInvoice.SetModified(userId);
                await _ksefInvoiceRepository.UpdateAsync(ksefInvoice, cancellationToken);
            }
        }

        return new EmptyBaseResponse();
    }
}
