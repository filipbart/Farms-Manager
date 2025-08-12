using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Common.Validators;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Hatcheries;

public record UpdateHatcheryDto
{
    public string Name { get; init; }
    public string ProducerNumber { get; init; }
    public string FullName { get; init; }
    public string Nip { get; init; }
    public string Address { get; init; }
}

public record UpdateHatcheryCommand(Guid Id, UpdateHatcheryDto Data) : IRequest<EmptyBaseResponse>;

public class UpdateHatcheryCommandHandler : IRequestHandler<UpdateHatcheryCommand, EmptyBaseResponse>
{
    private readonly IHatcheryRepository _hatcheryRepository;
    private readonly IUserDataResolver _userDataResolver;

    public UpdateHatcheryCommandHandler(IHatcheryRepository hatcheryRepository, IUserDataResolver userDataResolver)
    {
        _hatcheryRepository = hatcheryRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateHatcheryCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var hatcheryToUpdate = await _hatcheryRepository.GetAsync(new HatcheryByIdSpec(request.Id), cancellationToken);

        hatcheryToUpdate.Update(
            request.Data.Name,
            request.Data.ProducerNumber,
            request.Data.FullName,
            request.Data.Nip,
            request.Data.Address);

        hatcheryToUpdate.SetModified(userId);

        await _hatcheryRepository.UpdateAsync(hatcheryToUpdate, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}

public class UpdateHatcheryCommandValidator : AbstractValidator<UpdateHatcheryCommand>
{
    public UpdateHatcheryCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Data).NotNull();

        When(x => x.Data != null, () =>
        {
            RuleFor(x => x.Data.Name)
                .NotEmpty().WithMessage("Nazwa wylęgarni jest wymagana.")
                .MaximumLength(100);

            RuleFor(x => x.Data.ProducerNumber)
                .NotEmpty().WithMessage("Numer producenta jest wymagany.")
                .MaximumLength(50);

            RuleFor(x => x.Data.FullName)
                .NotEmpty().WithMessage("Pełna nazwa jest wymagana.")
                .MaximumLength(200);

            RuleFor(t => t.Data.Nip)
                .NotEmpty()
                .Must(ValidationHelpers.IsValidNip)
                .WithMessage("Podany numer NIP jest nieprawidłowy.");

            RuleFor(x => x.Data.Address)
                .MaximumLength(300);
        });
    }
}