using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Accounting;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Accounting;

public record DeleteAllKSeFInvoicesCommandResponse
{
    public int DeletedCount { get; init; }
}

public record DeleteAllKSeFInvoicesCommand : IRequest<BaseResponse<DeleteAllKSeFInvoicesCommandResponse>>;

public class DeleteAllKSeFInvoicesCommandHandler : IRequestHandler<DeleteAllKSeFInvoicesCommand,
    BaseResponse<DeleteAllKSeFInvoicesCommandResponse>>
{
    private readonly IKSeFInvoiceRepository _invoiceRepository;
    private readonly IUserDataResolver _userDataResolver;

    public DeleteAllKSeFInvoicesCommandHandler(
        IKSeFInvoiceRepository invoiceRepository,
        IUserDataResolver userDataResolver)
    {
        _invoiceRepository = invoiceRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<BaseResponse<DeleteAllKSeFInvoicesCommandResponse>> Handle(
        DeleteAllKSeFInvoicesCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        // Hard delete all invoices (for testing purposes only)
        var invoices = await _invoiceRepository.ListAsync(
            new AllActiveKSeFInvoicesSpec(),
            cancellationToken);

        var count = invoices.Count;

        foreach (var invoice in invoices)
        {
            await _invoiceRepository.DeleteAsync(invoice, cancellationToken);
        }

        await _invoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return BaseResponse.CreateResponse(new DeleteAllKSeFInvoicesCommandResponse
        {
            DeletedCount = count
        });
    }
}
