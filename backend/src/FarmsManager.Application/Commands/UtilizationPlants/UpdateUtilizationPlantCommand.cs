using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Common.Validators;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.UtilizationPlants;

public record UpdateUtilizationPlantDto(
    string Name,
    string IrzNumber,
    string Nip,
    string Address);

public record UpdateUtilizationPlantCommand(Guid UtilizationPlantId, UpdateUtilizationPlantDto Data)
    : IRequest<EmptyBaseResponse>;

public class UpdateUtilizationPlantCommandHandler : IRequestHandler<UpdateUtilizationPlantCommand, EmptyBaseResponse>
{
    private readonly IUtilizationPlantRepository _utilizationPlantRepository;
    private readonly IUserDataResolver _userDataResolver;

    public UpdateUtilizationPlantCommandHandler(IUtilizationPlantRepository utilizationPlantRepository,
        IUserDataResolver userDataResolver)
    {
        _utilizationPlantRepository = utilizationPlantRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateUtilizationPlantCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var utilizationPlant =
            await _utilizationPlantRepository.GetAsync(new UtilizationPlantByIdSpec(request.UtilizationPlantId),
                cancellationToken);

        utilizationPlant.Update(
            request.Data.Name,
            request.Data.IrzNumber,
            request.Data.Nip,
            request.Data.Address);

        utilizationPlant.SetModified(userId);

        await _utilizationPlantRepository.UpdateAsync(utilizationPlant, cancellationToken);

        return new EmptyBaseResponse();
    }
}

public class UpdateUtilizationPlantValidator : AbstractValidator<UpdateUtilizationPlantCommand>
{
    public UpdateUtilizationPlantValidator()
    {
        RuleFor(x => x.UtilizationPlantId).NotEmpty();
        RuleFor(x => x.Data).NotNull();

        When(x => x.Data != null, () =>
        {
            RuleFor(x => x.Data.Name)
                .NotEmpty().WithMessage("Nazwa ubojni jest wymagana.")
                .MaximumLength(100);

            RuleFor(x => x.Data.IrzNumber)
                .NotEmpty().WithMessage("Numer producenta jest wymagany.")
                .MaximumLength(50);

            RuleFor(t => t.Data.Nip)
                .NotEmpty()
                .Must(ValidationHelpers.IsValidNip)
                .WithMessage("Podany numer NIP jest nieprawidłowy.");

            RuleFor(x => x.Data.Address)
                .MaximumLength(300);
        });
    }
}