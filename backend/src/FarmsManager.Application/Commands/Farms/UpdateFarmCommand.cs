using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Common.Validators;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Farms;

public record UpdateFarmDto
{
    public string Name { get; init; }
    public string Nip { get; init; }
    public string ProducerNumber { get; init; }
    public string Address { get; init; }
}

public record UpdateFarmCommand(Guid Id, UpdateFarmDto Data) : IRequest<EmptyBaseResponse>;

public class UpdateFarmCommandHandler : IRequestHandler<UpdateFarmCommand, EmptyBaseResponse>
{
    private readonly IFarmRepository _farmRepository;
    private readonly IUserDataResolver _userDataResolver;

    public UpdateFarmCommandHandler(IFarmRepository farmRepository, IUserDataResolver userDataResolver)
    {
        _farmRepository = farmRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateFarmCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var farmToUpdate = await _farmRepository.GetAsync(new FarmByIdSpec(request.Id), cancellationToken);


        farmToUpdate.Update(
            request.Data.Name,
            request.Data.ProducerNumber,
            request.Data.Nip,
            request.Data.Address);

        farmToUpdate.SetModified(userId);

        await _farmRepository.UpdateAsync(farmToUpdate, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}

public class UpdateFarmCommandValidator : AbstractValidator<UpdateFarmCommand>
{
    public UpdateFarmCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Data).NotNull();

        When(x => x.Data != null, () =>
        {
            RuleFor(x => x.Data.Name)
                .NotEmpty().WithMessage("Nazwa jest wymagana.")
                .MaximumLength(100);

            RuleFor(t => t.Data.Nip)
                .NotEmpty()
                .Must(ValidationHelpers.IsValidNip)
                .WithMessage("Podany numer NIP jest nieprawidłowy.");

            RuleFor(x => x.Data.ProducerNumber)
                .NotEmpty().WithMessage("Numer producenta jest wymagany.")
                .MaximumLength(50);

            RuleFor(x => x.Data.Address)
                .MaximumLength(250);
        });
    }
}