using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Queries.Auth;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Users;

public record UpdateUserFarmsDto
{
    public List<Guid> FarmIds { get; init; } = [];
}

public record UpdateUserFarmsCommand(Guid UserId, List<Guid> FarmIds) : IRequest<EmptyBaseResponse>;

public class UpdateUserFarmsCommandHandler : IRequestHandler<UpdateUserFarmsCommand, EmptyBaseResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserFarmRepository _userFarmRepository;

    public UpdateUserFarmsCommandHandler(IUserRepository userRepository, IUserFarmRepository userFarmRepository)
    {
        _userRepository = userRepository;
        _userFarmRepository = userFarmRepository;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateUserFarmsCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetAsync(new UserByIdSpec(request.UserId), ct);

        await _userFarmRepository.DeleteRangeAsync(user.Farms, ct);

        foreach (var farmId in request.FarmIds)
        {
            if (user.Farms.Any(t => t.FarmId == farmId))
            {
                user.RemoveFarm(farmId);
            }
            else
            {
                user.AddFarm(farmId);
            }
        }

        await _userRepository.UpdateAsync(user, ct);
        return BaseResponse.EmptyResponse;
    }
}

public class UpdateUserFarmsCommandValidator : AbstractValidator<UpdateUserFarmsCommand>
{
    public UpdateUserFarmsCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.FarmIds).NotNull();
    }
}