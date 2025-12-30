using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Accounting;

/// <summary>
/// Command do masowej zmiany statusu faktur na "Przekazana do biura".
/// Używany gdy użytkownik zaznacza faktury zaakceptowane i klika "Przekaż do biura".
/// </summary>
public record TransferInvoicesToOfficeCommand(List<Guid> InvoiceIds) : IRequest<TransferInvoicesToOfficeResponse>;

public class TransferInvoicesToOfficeResponse
{
    public int TransferredCount { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class TransferInvoicesToOfficeCommandHandler : IRequestHandler<TransferInvoicesToOfficeCommand, TransferInvoicesToOfficeResponse>
{
    private readonly IKSeFInvoiceRepository _repository;
    private readonly IUserDataResolver _userDataResolver;

    public TransferInvoicesToOfficeCommandHandler(IKSeFInvoiceRepository repository, IUserDataResolver userDataResolver)
    {
        _repository = repository;
        _userDataResolver = userDataResolver;
    }

    public async Task<TransferInvoicesToOfficeResponse> Handle(TransferInvoicesToOfficeCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        
        var response = new TransferInvoicesToOfficeResponse();
        
        if (request.InvoiceIds == null || request.InvoiceIds.Count == 0)
        {
            response.Errors.Add("Nie wybrano żadnych faktur.");
            return response;
        }

        var invoices = await _repository.ListAsync(new KSeFInvoicesByIdsSpec(request.InvoiceIds), cancellationToken);
        
        foreach (var invoice in invoices)
        {
            // Tylko faktury "Zaakceptowane" mogą być przekazane do biura
            if (invoice.Status != KSeFInvoiceStatus.Accepted)
            {
                response.Errors.Add($"Faktura {invoice.InvoiceNumber} nie ma statusu 'Zaakceptowana' i została pominięta.");
                continue;
            }

            invoice.Update(status: KSeFInvoiceStatus.SentToOffice);
            invoice.SetModified(userId);
            response.TransferredCount++;
        }

        if (response.TransferredCount > 0)
        {
            await _repository.UpdateRangeAsync(invoices.Where(i => i.Status == KSeFInvoiceStatus.SentToOffice).ToList(), cancellationToken);
        }

        return response;
    }
}

public sealed class KSeFInvoicesByIdsSpec : BaseSpecification<KSeFInvoiceEntity>
{
    public KSeFInvoicesByIdsSpec(List<Guid> ids)
    {
        EnsureExists();
        Query.Where(i => ids.Contains(i.Id));
    }
}
