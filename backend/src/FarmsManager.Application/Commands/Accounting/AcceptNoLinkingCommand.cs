using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Accounting;

public record AcceptNoLinkingCommand(Guid InvoiceId) : IRequest<EmptyBaseResponse>;

public class AcceptNoLinkingCommandHandler : IRequestHandler<AcceptNoLinkingCommand, EmptyBaseResponse>
{
    private readonly IKSeFInvoiceRepository _invoiceRepository;
    private readonly IUserDataResolver _userDataResolver;

    public AcceptNoLinkingCommandHandler(IKSeFInvoiceRepository invoiceRepository, IUserDataResolver userDataResolver)
    {
        _invoiceRepository = invoiceRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(AcceptNoLinkingCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var spec = new AcceptNoLinkingSpec(request.InvoiceId);
        var invoice = await _invoiceRepository.GetAsync(spec, cancellationToken);

        invoice.AcceptNoLinking();
        invoice.SetModified(userId);
        await _invoiceRepository.UpdateAsync(invoice, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}

public class AcceptNoLinkingSpec : Specification<KSeFInvoiceEntity>, ISingleResultSpecification<KSeFInvoiceEntity>
{
    public AcceptNoLinkingSpec(Guid invoiceId)
    {
        Query.Where(x => x.Id == invoiceId && x.DateDeletedUtc == null);
    }
}
