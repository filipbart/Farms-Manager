using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Farms;

public record DeleteFarmCommand(Guid FarmId) : IRequest<EmptyBaseResponse>;

public class DeleteFarmCommandHandler : IRequestHandler<DeleteFarmCommand, EmptyBaseResponse>
{
    private readonly IFarmRepository _farmRepository;
    private readonly IUserDataResolver _userDataResolver;

    public DeleteFarmCommandHandler(IFarmRepository farmRepository, IUserDataResolver userDataResolver)
    {
        _farmRepository = farmRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteFarmCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.FarmId), cancellationToken);
        farm.Delete(userId);
        await _farmRepository.UpdateAsync(farm, cancellationToken);
        return new EmptyBaseResponse();
    }
}