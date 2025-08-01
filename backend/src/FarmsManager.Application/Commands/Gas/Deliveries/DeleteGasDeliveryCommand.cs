using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Gas;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FarmsManager.Shared.Extensions;
using MediatR;

namespace FarmsManager.Application.Commands.Gas.Deliveries;

public record DeleteGasDeliveryCommand(Guid Id) : IRequest<EmptyBaseResponse>;

public class DeleteGasDeliveryCommandHandler : IRequestHandler<DeleteGasDeliveryCommand, EmptyBaseResponse>
{
    private readonly IS3Service _s3Service;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IGasDeliveryRepository _gasDeliveryRepository;

    public DeleteGasDeliveryCommandHandler(IUserDataResolver userDataResolver,
        IGasDeliveryRepository gasDeliveryRepository, IS3Service s3Service)
    {
        _userDataResolver = userDataResolver;
        _gasDeliveryRepository = gasDeliveryRepository;
        _s3Service = s3Service;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteGasDeliveryCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var entity =
            await _gasDeliveryRepository.GetAsync(new GetGasDeliveryByIdSpec(request.Id),
                cancellationToken);

        if (entity.FilePath.IsNotEmpty())
        {
            await _s3Service.DeleteFileAsync(FileType.GasDelivery, entity.FilePath);
        }

        entity.Delete(userId);
        await _gasDeliveryRepository.UpdateAsync(entity, cancellationToken);
        return BaseResponse.EmptyResponse;
    }
}