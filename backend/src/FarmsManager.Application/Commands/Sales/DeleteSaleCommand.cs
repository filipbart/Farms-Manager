using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Sales;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Sales;

public record DeleteSaleCommand(Guid Id) : IRequest<EmptyBaseResponse>;

public class DeleteSaleCommandHandler : IRequestHandler<DeleteSaleCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly ISaleRepository _saleRepository;
    private readonly IS3Service _s3Service;

    public DeleteSaleCommandHandler(IUserDataResolver userDataResolver, ISaleRepository saleRepository,
        IS3Service s3Service)
    {
        _userDataResolver = userDataResolver;
        _saleRepository = saleRepository;
        _s3Service = s3Service;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var sale = await _saleRepository.GetAsync(new SaleByIdSpec(request.Id), cancellationToken);

        await _s3Service.DeleteFolderAsync(FileType.Sales, sale.DirectoryPath);

        sale.Delete(userId);
        await _saleRepository.UpdateAsync(sale, cancellationToken);

        return new EmptyBaseResponse();
    }
}