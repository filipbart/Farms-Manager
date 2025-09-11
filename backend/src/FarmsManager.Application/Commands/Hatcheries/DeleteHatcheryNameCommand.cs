using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Interfaces;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Hatcheries;

public record DeleteHatcheryNameCommand(Guid Id) : IRequest<EmptyBaseResponse>;

public class DeleteHatcheryNameCommandValidator : AbstractValidator<DeleteHatcheryNameCommand>
{
    public DeleteHatcheryNameCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id jest wymagane.");
    }
}

public class DeleteHatcheryNameCommandHandler : IRequestHandler<DeleteHatcheryNameCommand, EmptyBaseResponse>
{
    private readonly IHatcheryNameRepository _hatcheryNameRepository;
    private readonly IHatcheryPriceRepository _hatcheryPriceRepository;

    public DeleteHatcheryNameCommandHandler(IHatcheryNameRepository hatcheryNameRepository, IHatcheryPriceRepository hatcheryPriceRepository)
    {
        _hatcheryNameRepository = hatcheryNameRepository;
        _hatcheryPriceRepository = hatcheryPriceRepository;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteHatcheryNameCommand request, CancellationToken cancellationToken)
    {
        var hatcheryName = await _hatcheryNameRepository.GetByIdAsync(request.Id, cancellationToken);
        if (hatcheryName is null)
        {
            throw new Exception("Nie znaleziono wybranej wylęgarni.");
        }

        var isUsed = await _hatcheryPriceRepository.AnyAsync(new HatcheryPriceByHatcheryNameSpec(hatcheryName.Name), cancellationToken);
        if (isUsed)
        {
            throw new Exception("Nie można usunąć nazwy wylęgarni, ponieważ jest używana w cenach.");
        }

        await _hatcheryNameRepository.DeleteAsync(hatcheryName, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}

public sealed class HatcheryPriceByHatcheryNameSpec : Specification<HatcheryPriceEntity>
{
    public HatcheryPriceByHatcheryNameSpec(string hatcheryName)
    {
        Query.Where(x => x.HatcheryName == hatcheryName);
    }
}