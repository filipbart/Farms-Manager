using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Accounting;

/// <summary>
/// Command do wstrzymania faktury i przydzielenia jej do innego pracownika.
/// Nie zmienia statusu faktury - pozostaje jako "Nowa".
/// </summary>
public record HoldKSeFInvoiceCommand(Guid InvoiceId, HoldKSeFInvoiceDto Data) : IRequest<EmptyBaseResponse>;

public class HoldKSeFInvoiceDto
{
    /// <summary>
    /// Nowy pracownik, do którego ma być przypisana faktura
    /// </summary>
    public Guid NewAssignedUserId { get; set; }
    
    /// <summary>
    /// Aktualnie przypisany pracownik (do walidacji czy nie zmienił się przed kliknięciem)
    /// </summary>
    public Guid? ExpectedCurrentAssignedUserId { get; set; }
}

public class HoldKSeFInvoiceCommandHandler : IRequestHandler<HoldKSeFInvoiceCommand, EmptyBaseResponse>
{
    private readonly IKSeFInvoiceRepository _repository;
    private readonly IUserDataResolver _userDataResolver;

    public HoldKSeFInvoiceCommandHandler(IKSeFInvoiceRepository repository, IUserDataResolver userDataResolver)
    {
        _repository = repository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(HoldKSeFInvoiceCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var invoice = await _repository.GetAsync(new KSeFInvoiceByIdSpec(request.InvoiceId), cancellationToken);

        // Walidacja: sprawdź czy przypisany pracownik nie zmienił się od czasu otwarcia modala
        if (invoice.AssignedUserId != request.Data.ExpectedCurrentAssignedUserId)
        {
            throw DomainException.BadRequest(
                "Przypisany pracownik został zmieniony przez innego użytkownika. Odśwież stronę i spróbuj ponownie.");
        }

        // Nadpisz przypisanego pracownika (nie zmieniaj statusu)
        invoice.Update(assignedUserId: request.Data.NewAssignedUserId);

        invoice.SetModified(userId);
        await _repository.UpdateAsync(invoice, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}
