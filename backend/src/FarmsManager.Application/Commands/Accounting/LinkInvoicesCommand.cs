using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Accounting;

public record LinkInvoicesCommand(LinkInvoicesDto Data) : IRequest<EmptyBaseResponse>;

public class LinkInvoicesDto
{
    public Guid SourceInvoiceId { get; set; }
    public List<Guid> TargetInvoiceIds { get; set; } = [];
    public InvoiceRelationType RelationType { get; set; }
}

public class LinkInvoicesCommandHandler : IRequestHandler<LinkInvoicesCommand, EmptyBaseResponse>
{
    private readonly IKSeFInvoiceRepository _invoiceRepository;
    private readonly IKSeFInvoiceRelationRepository _relationRepository;
    private readonly IUserDataResolver _userDataResolver;

    public LinkInvoicesCommandHandler(
        IKSeFInvoiceRepository invoiceRepository,
        IKSeFInvoiceRelationRepository relationRepository,
        IUserDataResolver userDataResolver)
    {
        _invoiceRepository = invoiceRepository;
        _relationRepository = relationRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(LinkInvoicesCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var spec = new LinkInvoicesSpec(request.Data.SourceInvoiceId);
        var sourceInvoice = await _invoiceRepository.GetAsync(spec, cancellationToken);

        foreach (var targetInvoiceId in request.Data.TargetInvoiceIds)
        {
            var relation = KSeFInvoiceRelationEntity.CreateNew(
                sourceInvoiceId: request.Data.SourceInvoiceId,
                targetInvoiceId: targetInvoiceId,
                relationType: request.Data.RelationType,
                userId: userId
            );

            await _relationRepository.AddAsync(relation, cancellationToken);
        }

        sourceInvoice.MarkAsLinked();
        sourceInvoice.SetModified(userId);
        await _invoiceRepository.UpdateAsync(sourceInvoice, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}

public class LinkInvoicesSpec : Specification<KSeFInvoiceEntity>, ISingleResultSpecification<KSeFInvoiceEntity>
{
    public LinkInvoicesSpec(Guid invoiceId)
    {
        Query.Where(x => x.Id == invoiceId && x.DateDeletedUtc == null);
    }
}
