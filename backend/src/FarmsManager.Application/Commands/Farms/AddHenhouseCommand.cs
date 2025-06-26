using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Farms;

public record AddHenhouseCommand(Guid FarmId, string Name, string Code, int Area, string Desc)
    : IRequest<EmptyBaseResponse>;

public class AddHenhouseCommandHandler : IRequestHandler<AddHenhouseCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFarmRepository _farmRepository;
    private readonly IHenhouseRepository _henhouseRepository;

    public AddHenhouseCommandHandler(IUserDataResolver userDataResolver, IFarmRepository farmRepository,
        IHenhouseRepository henhouseRepository)
    {
        _userDataResolver = userDataResolver;
        _farmRepository = farmRepository;
        _henhouseRepository = henhouseRepository;
    }

    public async Task<EmptyBaseResponse> Handle(AddHenhouseCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.FarmId), cancellationToken);

        var henhouse =
            HenhouseEntity.CreateNew(request.Name, request.Code, request.Area, request.Desc, farm.Id, userId);
        await _henhouseRepository.AddAsync(henhouse, cancellationToken);
        return new EmptyBaseResponse();
    }
}

public class AddHenhouseCommandValidator : AbstractValidator<AddHenhouseCommand>
{
    public AddHenhouseCommandValidator()
    {
        RuleFor(t => t.Name).NotEmpty();
        RuleFor(t => t.Area).GreaterThanOrEqualTo(0);
    }
}