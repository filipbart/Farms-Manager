using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.ProductionData.FlockLosses;

public record DeleteProductionDataFlockLossCommand(Guid Id) : IRequest<EmptyBaseResponse>;

public class
    DeleteProductionDataFlockLossCommandHandler : IRequestHandler<DeleteProductionDataFlockLossCommand,
    EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IProductionDataFlockLossMeasureRepository _repository;

    public DeleteProductionDataFlockLossCommandHandler(IUserDataResolver userDataResolver,
        IProductionDataFlockLossMeasureRepository repository)
    {
        _userDataResolver = userDataResolver;
        _repository = repository;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteProductionDataFlockLossCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var flockLossMeasure =
            await _repository.GetAsync(new ProductionDataFlockLossMeasureByIdSpec(request.Id),
                cancellationToken);

        flockLossMeasure.Delete(userId);
        await _repository.UpdateAsync(flockLossMeasure, cancellationToken);

        return new EmptyBaseResponse();
    }
}