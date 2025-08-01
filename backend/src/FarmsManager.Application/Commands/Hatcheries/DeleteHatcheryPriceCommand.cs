using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Hatcheries;

public record DeleteHatcheryPriceCommand(Guid Id) : IRequest<EmptyBaseResponse>;

public class DeleteHatcheryPriceCommandValidator : AbstractValidator<DeleteHatcheryPriceCommand>
{
    public DeleteHatcheryPriceCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID ceny jest wymagane.");
    }
}

public class DeleteHatcheryPriceCommandHandler : IRequestHandler<DeleteHatcheryPriceCommand, EmptyBaseResponse>
{
    private readonly IHatcheryPriceRepository _hatcheryPriceRepository;
    private readonly IUserDataResolver _userDataResolver;

    public DeleteHatcheryPriceCommandHandler(IHatcheryPriceRepository hatcheryPriceRepository,
        IUserDataResolver userDataResolver)
    {
        _hatcheryPriceRepository = hatcheryPriceRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteHatcheryPriceCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var hatcheryPrice =
            await _hatcheryPriceRepository.GetAsync(new HatcheryPriceByIdSpec(request.Id), cancellationToken);

        hatcheryPrice.Delete(userId);

        await _hatcheryPriceRepository.UpdateAsync(hatcheryPrice, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}