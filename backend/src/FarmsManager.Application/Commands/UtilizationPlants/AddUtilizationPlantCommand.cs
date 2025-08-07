using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Common.Validators;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Entities;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.UtilizationPlants;

public class AddUtilizationPlantCommand : IRequest<EmptyBaseResponse>
{
    public string Name { get; init; }
    public string IrzNumber { get; init; }
    public string Nip { get; init; }
    public string Address { get; init; }
}

public class AddUtilizationPlantCommandValidator : AbstractValidator<AddUtilizationPlantCommand>
{
    public AddUtilizationPlantCommandValidator()
    {
        RuleFor(t => t.Name).NotEmpty();
        RuleFor(t => t.IrzNumber).NotEmpty();
        RuleFor(t => t.Nip).NotEmpty()
            .Must(ValidationHelpers.IsValidNip)
            .WithMessage("Podany numer NIP jest nieprawidłowy.");
    }
}

public class AddUtilizationPlantCommandHandler : IRequestHandler<AddUtilizationPlantCommand, EmptyBaseResponse>
{
    private readonly IUtilizationPlantRepository _utilizationPlantRepository;
    private readonly IUserDataResolver _userDataResolver;

    public AddUtilizationPlantCommandHandler(IUtilizationPlantRepository utilizationPlantRepository,
        IUserDataResolver userDataResolver)
    {
        _utilizationPlantRepository = utilizationPlantRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(AddUtilizationPlantCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();


        var newPlant = UtilizationPlantEntity.CreateNew(request.Name, request.IrzNumber, request.Nip,
            request.Address, userId);

        await _utilizationPlantRepository.AddAsync(newPlant, cancellationToken);

        return new EmptyBaseResponse();
    }
}