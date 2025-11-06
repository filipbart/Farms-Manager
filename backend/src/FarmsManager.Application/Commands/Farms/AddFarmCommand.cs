using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Common.Validators;
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
    public string ProdNumber { get; init; }
    public string Nip { get; init; }
    public string Address { get; init; }
}

public class AddFarmCommandValidator : AbstractValidator<AddFarmCommand>
{
    public AddFarmCommandValidator()
    {
        RuleFor(t => t.Name).NotEmpty();
        RuleFor(t => t.ProdNumber)
            .NotEmpty().WithMessage("Numer producenta jest wymagany.")
            .Must(ValidationHelpers.IsValidProducerOrIrzNumber)
            .WithMessage("Numer producenta musi być w formacie liczba-liczba (np. 00011233-123).");
        RuleFor(t => t.Nip).NotEmpty().Must(ValidationHelpers.IsValidNip)
            .WithMessage("Podany numer NIP jest nieprawidłowy.");
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
        var newFarm = FarmEntity.CreateNew(request.Name, request.ProdNumber, request.Nip, request.Address, userId);
        await _farmRepository.AddAsync(newFarm, cancellationToken);

        return new EmptyBaseResponse();
    }
}