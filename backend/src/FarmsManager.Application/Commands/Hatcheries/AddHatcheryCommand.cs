using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Common.Validators;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Hatcheries;

public class AddHatcheryCommand : IRequest<EmptyBaseResponse>
{
    /// <summary>
    /// Nazwa
    /// </summary>
    public string Name { get; init; }
    
    /// <summary>
    /// Numer producenta
    /// </summary>
    public string ProdNumber { get; init; }
    
    /// <summary>
    /// Pełna nazwa
    /// </summary>
    public string FullName { get; init; }
    
    /// <summary>
    /// NIP
    /// </summary>
    public string Nip { get; init; }
    
    /// <summary>
    /// Adres
    /// </summary>
    public string Address { get; init; }
}

public class AddHatcheryCommandValidator : AbstractValidator<AddHatcheryCommand>
{
    public AddHatcheryCommandValidator()
    {
        RuleFor(t => t.Name).NotEmpty();
        RuleFor(t => t.FullName).NotEmpty();
        RuleFor(t => t.Nip).NotEmpty().Must(ValidationHelpers.IsValidNip)
            .WithMessage("Podany numer NIP jest nieprawidłowy.");
    }
}

public class AddHatcheryCommandHandler : IRequestHandler<AddHatcheryCommand, EmptyBaseResponse>
{
    private readonly IHatcheryRepository _hatcheryRepository;
    private readonly IUserDataResolver _userDataResolver;

    public AddHatcheryCommandHandler(IHatcheryRepository hatcheryRepository, IUserDataResolver userDataResolver)
    {
        _hatcheryRepository = hatcheryRepository;
        _userDataResolver = userDataResolver;
    }


    public async Task<EmptyBaseResponse> Handle(AddHatcheryCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var newHatcher = HatcheryEntity.CreateNew(request.Name, request.ProdNumber, request.FullName, request.Nip,
            request.Address, userId);
        await _hatcheryRepository.AddAsync(newHatcher, cancellationToken);

        return new EmptyBaseResponse();
    }
}