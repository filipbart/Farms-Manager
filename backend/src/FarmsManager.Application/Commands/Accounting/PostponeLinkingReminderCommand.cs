using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Accounting;

public record PostponeLinkingReminderCommand(Guid InvoiceId, int Days = 3) : IRequest<EmptyBaseResponse>;

public class PostponeLinkingReminderCommandHandler : IRequestHandler<PostponeLinkingReminderCommand, EmptyBaseResponse>
{
    private readonly IKSeFInvoiceRepository _invoiceRepository;
    private readonly IUserDataResolver _userDataResolver;

    public PostponeLinkingReminderCommandHandler(IKSeFInvoiceRepository invoiceRepository, IUserDataResolver userDataResolver)
    {
        _invoiceRepository = invoiceRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(PostponeLinkingReminderCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var spec = new PostponeLinkingReminderSpec(request.InvoiceId);
        var invoice = await _invoiceRepository.GetAsync(spec, cancellationToken);

        invoice.PostponeLinkingReminder(request.Days);
        invoice.SetModified(userId);
        await _invoiceRepository.UpdateAsync(invoice, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}

public class PostponeLinkingReminderSpec : Specification<KSeFInvoiceEntity>, ISingleResultSpecification<KSeFInvoiceEntity>
{
    public PostponeLinkingReminderSpec(Guid invoiceId)
    {
        Query.Where(x => x.Id == invoiceId && x.DateDeletedUtc == null);
    }
}
