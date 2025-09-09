using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Entities;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Farms;

public record DeleteFarmCommand(Guid FarmId) : IRequest<EmptyBaseResponse>;

public class DeleteFarmCommandHandler : IRequestHandler<DeleteFarmCommand, EmptyBaseResponse>
{
    private readonly IFarmRepository _farmRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;

    public DeleteFarmCommandHandler(IFarmRepository farmRepository, IUserDataResolver userDataResolver,
        IUserRepository userRepository)
    {
        _farmRepository = farmRepository;
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteFarmCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.FarmId), cancellationToken);

        var users = await _userRepository.ListAsync(new GetAllUsersSpec(), cancellationToken);
        foreach (var user in users)
        {
            user.NotificationFarms.Remove(farm.Id);
            user.AccessibleFarmIds.Remove(farm.Id);
            var farmCredentials = user.IrzplusCredentials?.FirstOrDefault(c => c.FarmId == farm.Id);
            if (farmCredentials != null)
            {
                user.RemoveIrzplusCredentials(farmCredentials);
                await _userRepository.UpdateAsync(user, cancellationToken);
            }
        }

        farm.Delete(userId);
        await _farmRepository.UpdateAsync(farm, cancellationToken);
        return new EmptyBaseResponse();
    }
}

public sealed class GetAllUsersSpec : BaseSpecification<UserEntity>
{
    public GetAllUsersSpec()
    {
        EnsureExists();
    }
}