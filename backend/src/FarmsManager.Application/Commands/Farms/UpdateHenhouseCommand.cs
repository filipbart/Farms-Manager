using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Farms;

public record UpdateHenhouseDto
{
    public string Name { get; init; }
    public string Code { get; init; }
    public int Area { get; init; }
    public string Description { get; init; }
}

public record UpdateHenhouseCommand(Guid Id, UpdateHenhouseDto Data) : IRequest<EmptyBaseResponse>;

public class UpdateHenhouseCommandHandler : IRequestHandler<UpdateHenhouseCommand, EmptyBaseResponse>
{
    private readonly IHenhouseRepository _henhouseRepository;
    private readonly IUserDataResolver _userDataResolver;

    public UpdateHenhouseCommandHandler(IHenhouseRepository henhouseRepository, IUserDataResolver userDataResolver)
    {
        _henhouseRepository = henhouseRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateHenhouseCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var henhouseToUpdate = await _henhouseRepository.GetAsync(new HenhouseByIdSpec(request.Id), cancellationToken);

        henhouseToUpdate.Update(
            request.Data.Name,
            request.Data.Code,
            request.Data.Area,
            request.Data.Description);

        henhouseToUpdate.SetModified(userId);

        await _henhouseRepository.UpdateAsync(henhouseToUpdate, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}

public class UpdateHenhouseCommandValidator : AbstractValidator<UpdateHenhouseCommand>
{
    public UpdateHenhouseCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Data).NotNull();

        When(x => x.Data != null, () =>
        {
            RuleFor(x => x.Data.Name)
                .NotEmpty().WithMessage("Nazwa kurnika jest wymagana.")
                .MaximumLength(100);

            RuleFor(x => x.Data.Code)
                .NotEmpty().WithMessage("ID budynku jest wymagane.")
                .MaximumLength(50);

            RuleFor(x => x.Data.Area)
                .GreaterThan(0).WithMessage("Powierzchnia musi być większa od 0.");

            RuleFor(x => x.Data.Description)
                .MaximumLength(500);
        });
    }
}