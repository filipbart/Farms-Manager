using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Sales;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Sales.Invoices;

public record MarkSaleInvoiceAsUnrealizedCommand(Guid Id) : IRequest<EmptyBaseResponse>;

public class MarkSaleInvoiceAsUnrealizedCommandHandler 
    : IRequestHandler<MarkSaleInvoiceAsUnrealizedCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly ISaleInvoiceRepository _saleInvoiceRepository;

    public MarkSaleInvoiceAsUnrealizedCommandHandler(
        IUserDataResolver userDataResolver,
        ISaleInvoiceRepository saleInvoiceRepository)
    {
        _userDataResolver = userDataResolver;
        _saleInvoiceRepository = saleInvoiceRepository;
    }

    public async Task<EmptyBaseResponse> Handle(
        MarkSaleInvoiceAsUnrealizedCommand request, 
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        
        var saleInvoice = await _saleInvoiceRepository.GetAsync(
            new SaleInvoiceByIdSpec(request.Id), 
            cancellationToken);

        saleInvoice.MarkAsUnrealized();
        saleInvoice.SetModified(userId);
        
        await _saleInvoiceRepository.UpdateAsync(saleInvoice, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}
