using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Farms;

public record AddFarmCommand : IRequest<EmptyBaseResponse>
{
    public string Name { get; init; }
    public string Nip { get; init; }
    public string Address { get; init; }
}

public class AddFarmCommandValidator : AbstractValidator<AddFarmCommand>
{
    public AddFarmCommandValidator()
    {
        RuleFor(t => t.Name).NotEmpty();
        RuleFor(t => t.Nip).NotEmpty();
        RuleFor(t => t.Address).NotEmpty();
    }
}

public class AddFarmCommandHandler : IRequestHandler<AddFarmCommand, EmptyBaseResponse>
{
    private readonly IFarmRepository _farmRepository;
    private readonly IUserDataResolver _userDataResolver;

    public AddFarmCommandHandler(IFarmRepository farmRepository, IUserDataResolver userDataResolver)
    {
        _farmRepository = farmRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(AddFarmCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var newFarm = FarmEntity.CreateNew(request.Name, request.Nip, request.Address, userId);
        await _farmRepository.AddAsync(newFarm, cancellationToken);

        return new EmptyBaseResponse();
    }
}