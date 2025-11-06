using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Common.Validators;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.SlaughterhouseAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Slaughterhouses;

public record UpdateSlaughterhouseDto
{
    public string Name { get; init; }
    public string ProducerNumber { get; init; }
    public string Nip { get; init; }
    public string Address { get; init; }
}

public record UpdateSlaughterhouseCommand(Guid Id, UpdateSlaughterhouseDto Data) : IRequest<EmptyBaseResponse>;

public class UpdateSlaughterhouseCommandHandler : IRequestHandler<UpdateSlaughterhouseCommand, EmptyBaseResponse>
{
    private readonly ISlaughterhouseRepository _slaughterhouseRepository;
    private readonly IUserDataResolver _userDataResolver;

    public UpdateSlaughterhouseCommandHandler(ISlaughterhouseRepository slaughterhouseRepository,
        IUserDataResolver userDataResolver)
    {
        _slaughterhouseRepository = slaughterhouseRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateSlaughterhouseCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var slaughterhouseToUpdate =
            await _slaughterhouseRepository.GetAsync(new SlaughterhouseByIdSpec(request.Id), cancellationToken);

        slaughterhouseToUpdate.Update(
            request.Data.Name,
            request.Data.ProducerNumber,
            request.Data.Nip,
            request.Data.Address);

        slaughterhouseToUpdate.SetModified(userId);

        await _slaughterhouseRepository.UpdateAsync(slaughterhouseToUpdate, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}

public class UpdateSlaughterhouseCommandValidator : AbstractValidator<UpdateSlaughterhouseCommand>
{
    public UpdateSlaughterhouseCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Data).NotNull();

        When(x => x.Data != null, () =>
        {
            RuleFor(x => x.Data.Name)
                .NotEmpty().WithMessage("Nazwa ubojni jest wymagana.")
                .MaximumLength(100);

            RuleFor(x => x.Data.ProducerNumber)
                .NotEmpty().WithMessage("Numer producenta jest wymagany.")
                .Must(ValidationHelpers.IsValidProducerOrIrzNumber)
                .WithMessage("Numer producenta musi być w formacie liczba-liczba (np. 00011233-123).")
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