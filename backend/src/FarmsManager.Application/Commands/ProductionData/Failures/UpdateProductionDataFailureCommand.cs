using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.ProductionData;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;


namespace FarmsManager.Application.Commands.ProductionData.Failures;

public record UpdateProductionDataFailureCommandDto
{
    public int DeadCount { get; init; }
    public int DefectiveCount { get; init; }
}

public record UpdateProductionDataFailureCommand(Guid Id, UpdateProductionDataFailureCommandDto Data)
    : IRequest<EmptyBaseResponse>;

public class
    UpdateProductionDataFailureCommandHandler : IRequestHandler<UpdateProductionDataFailureCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IProductionDataFailureRepository _failureRepository;

    public UpdateProductionDataFailureCommandHandler(IUserDataResolver userDataResolver,
        IProductionDataFailureRepository failureRepository)
    {
        _userDataResolver = userDataResolver;
        _failureRepository = failureRepository;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateProductionDataFailureCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var failure =
            await _failureRepository.GetAsync(new ProductionDataFailureByIdSpec(request.Id), cancellationToken);

        failure.UpdateData(request.Data.DeadCount, request.Data.DefectiveCount);
        failure.SetModified(userId);
        await _failureRepository.UpdateAsync(failure, cancellationToken);

        return new EmptyBaseResponse();
    }
}