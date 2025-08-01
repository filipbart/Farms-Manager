using FarmsManager.Application.Common.Responses;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Interfaces;
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

    public DeleteHatcheryPriceCommandHandler(IHatcheryPriceRepository hatcheryPriceRepository)
    {
        _hatcheryPriceRepository = hatcheryPriceRepository;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteHatcheryPriceCommand request, CancellationToken cancellationToken)
    {
        var hatcheryPrice =
            await _hatcheryPriceRepository.GetAsync(new HatcheryPriceByIdSpec(request.Id), cancellationToken);

        hatcheryPrice.Delete();

        await _hatcheryPriceRepository.UpdateAsync(hatcheryPrice, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}