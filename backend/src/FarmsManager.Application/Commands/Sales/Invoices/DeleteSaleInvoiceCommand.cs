using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Sales;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FarmsManager.Shared.Extensions;
using MediatR;

namespace FarmsManager.Application.Commands.Sales.Invoices;

public record DeleteSaleInvoiceCommand(Guid Id) : IRequest<EmptyBaseResponse>;

public class DeleteSalesInvoiceCommandHandler : IRequestHandler<DeleteSaleInvoiceCommand, EmptyBaseResponse>
{
    private readonly IS3Service _s3Service;
    private readonly IUserDataResolver _userDataResolver;
    private readonly ISaleInvoiceRepository _saleInvoiceRepository;

    public DeleteSalesInvoiceCommandHandler(IUserDataResolver userDataResolver,
        ISaleInvoiceRepository saleInvoiceRepository, IS3Service s3Service)
    {
        _userDataResolver = userDataResolver;
        _saleInvoiceRepository = saleInvoiceRepository;
        _s3Service = s3Service;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteSaleInvoiceCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var entity =
            await _saleInvoiceRepository.GetAsync(new SaleInvoiceByIdSpec(request.Id),
                cancellationToken);

        if (entity.FilePath.IsNotEmpty())
        {
            await _s3Service.DeleteFileAsync(FileType.SalesInvoices, entity.FilePath);
        }

        entity.Delete(userId);
        await _saleInvoiceRepository.UpdateAsync(entity, cancellationToken);
        return BaseResponse.EmptyResponse;
    }
}