using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.ProductionData;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.ProductionData.Weighings;

public record DeleteProductionDataWeighingCommand(Guid Id) : IRequest<EmptyBaseResponse>;

public class
    DeleteProductionDataWeighingCommandHandler : IRequestHandler<DeleteProductionDataWeighingCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IProductionDataWeighingRepository _repository;

    public DeleteProductionDataWeighingCommandHandler(IUserDataResolver userDataResolver,
        IProductionDataWeighingRepository repository)
    {
        _userDataResolver = userDataResolver;
        _repository = repository;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteProductionDataWeighingCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var weighing =
            await _repository.GetAsync(new ProductionDataWeighingByIdSpec(request.Id),
                cancellationToken);

        weighing.Delete(userId);
        await _repository.UpdateAsync(weighing, cancellationToken);

        return new EmptyBaseResponse();
    }
}