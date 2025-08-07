using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Entities;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.UtilizationPlants;

public record DeleteUtilizationPlantCommand(Guid UtilizationPlantId) : IRequest<EmptyBaseResponse>;

public class DeleteUtilizationPlantCommandHandler : IRequestHandler<DeleteUtilizationPlantCommand, EmptyBaseResponse>
{
    private readonly IUtilizationPlantRepository _utilizationPlantRepository;
    private readonly IUserDataResolver _userDataResolver;

    public DeleteUtilizationPlantCommandHandler(IUtilizationPlantRepository utilizationPlantRepository,
        IUserDataResolver userDataResolver)
    {
        _utilizationPlantRepository = utilizationPlantRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteUtilizationPlantCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var utilizationPlant =
            await _utilizationPlantRepository.GetAsync(new UtilizationPlantByIdSpec(request.UtilizationPlantId),
                cancellationToken);

        utilizationPlant.Delete(userId);

        await _utilizationPlantRepository.UpdateAsync(utilizationPlant, cancellationToken);

        return new EmptyBaseResponse();
    }
}

public sealed class UtilizationPlantByIdSpec : BaseSpecification<UtilizationPlantEntity>,
    ISingleResultSpecification<UtilizationPlantEntity>
{
    public UtilizationPlantByIdSpec(Guid id)
    {
        EnsureExists();
        Query.Where(t => t.Id == id);
    }
}