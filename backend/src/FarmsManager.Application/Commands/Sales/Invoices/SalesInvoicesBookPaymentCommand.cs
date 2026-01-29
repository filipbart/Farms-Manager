using Ardalis.Specification;
using FarmsManager.Application.Commands.Accounting;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Sales.Invoices;

public record SalesInvoicesBookPaymentCommand : IRequest<EmptyBaseResponse>
{
    public List<Guid> InvoicesIds { get; init; } = [];
    public DateOnly PaymentDate { get; init; }
}

public class
    SalesInvoicesBookPaymentCommandHandler : IRequestHandler<SalesInvoicesBookPaymentCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly ISaleInvoiceRepository _saleInvoiceRepository;
    private readonly IKSeFInvoiceRepository _ksefInvoiceRepository;

    public SalesInvoicesBookPaymentCommandHandler(
        IUserDataResolver userDataResolver,
        ISaleInvoiceRepository saleInvoiceRepository,
        IKSeFInvoiceRepository ksefInvoiceRepository)
    {
        _userDataResolver = userDataResolver;
        _saleInvoiceRepository = saleInvoiceRepository;
        _ksefInvoiceRepository = ksefInvoiceRepository;
    }

    public async Task<EmptyBaseResponse> Handle(SalesInvoicesBookPaymentCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var spec = new GetSalesInvoicesByIdsSpec(request.InvoicesIds);
        var invoices = await _saleInvoiceRepository.ListAsync(spec, cancellationToken);

        if (invoices.Count != request.InvoicesIds.Count)
        {
            throw new Exception("Nie znaleziono wszystkich faktur o podanych identyfikatorach.");
        }

        foreach (var invoice in invoices)
        {
            invoice.BookPayment(request.PaymentDate);
            invoice.SetModified(userId);
        }

        await _saleInvoiceRepository.UpdateRangeAsync(invoices, cancellationToken);

        // Synchronizacja statusu płatności do księgowości (KSeF)
        foreach (var saleInvoice in invoices)
        {
            var ksefInvoice = await _ksefInvoiceRepository.FirstOrDefaultAsync(
                new KSeFInvoiceByAssignedEntityIdSpec(saleInvoice.Id),
                cancellationToken);

            if (ksefInvoice != null)
            {
                ksefInvoice.Update(paymentStatus: KSeFPaymentStatus.PaidTransfer);
                ksefInvoice.SetModified(userId);
                await _ksefInvoiceRepository.UpdateAsync(ksefInvoice, cancellationToken);
            }
        }

        return BaseResponse.EmptyResponse;
    }
}

public class SalesInvoicesBookPaymentCommandValidator : AbstractValidator<SalesInvoicesBookPaymentCommand>
{
    public SalesInvoicesBookPaymentCommandValidator()
    {
        RuleFor(x => x.InvoicesIds).NotEmpty().WithMessage("Należy wybrać przynajmniej jedną fakturę.");
        RuleFor(x => x.PaymentDate).NotEmpty().WithMessage("Data płatności jest wymagana.");
    }
}

public sealed class GetSalesInvoicesByIdsSpec : BaseSpecification<SaleInvoiceEntity>
{
    public GetSalesInvoicesByIdsSpec(List<Guid> invoicesIds)
    {
        EnsureExists();
        Query.Where(t => invoicesIds.Contains(t.Id));
    }
}