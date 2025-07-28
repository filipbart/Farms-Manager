using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.ProductionData.Failures;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;


namespace FarmsManager.Application.Commands.ProductionData.Failures;

public record DeleteProductionDataFailureCommand(Guid Id) : IRequest<EmptyBaseResponse>;

public class
    DeleteProductionDataFailureCommandHandler : IRequestHandler<DeleteProductionDataFailureCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IProductionDataFailureRepository _failureRepository;

    public DeleteProductionDataFailureCommandHandler(IUserDataResolver userDataResolver,
        IProductionDataFailureRepository failureRepository)
    {
        _userDataResolver = userDataResolver;
        _failureRepository = failureRepository;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteProductionDataFailureCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var failure =
            await _failureRepository.GetAsync(new ProductionDataFailureByIdSpec(request.Id),
                cancellationToken);

        failure.Delete(userId);
        await _failureRepository.UpdateAsync(failure, cancellationToken);

        return new EmptyBaseResponse();
    }
}