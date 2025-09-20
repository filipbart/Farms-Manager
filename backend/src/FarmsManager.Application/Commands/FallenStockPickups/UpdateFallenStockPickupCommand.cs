using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.FallenStockPickups;

public record UpdateFallenStockPickupDto
{
    public Guid CycleId { get; set; }
    public int Quantity { get; set; }
}

public record UpdateFallenStockPickupCommand(Guid FallenStockPickupId, UpdateFallenStockPickupDto Data)
    : IRequest<EmptyBaseResponse>;

public class UpdateFallenStockPickupCommandHandler
    : IRequestHandler<UpdateFallenStockPickupCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFallenStockPickupRepository _fallenStockPickupRepository;
    private readonly ICycleRepository _cycleRepository;

    public UpdateFallenStockPickupCommandHandler(
        IUserDataResolver userDataResolver,
        IFallenStockPickupRepository fallenStockPickupRepository,
        ICycleRepository cycleRepository)
    {
        _userDataResolver = userDataResolver;
        _fallenStockPickupRepository = fallenStockPickupRepository;
        _cycleRepository = cycleRepository;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateFallenStockPickupCommand request, CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var response = new EmptyBaseResponse();

        var pickup =
            await _fallenStockPickupRepository.GetAsync(new FallenStockPickupByIdSpec(request.FallenStockPickupId), ct);

        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(request.Data.CycleId), ct);

        if (pickup.CycleId != cycle.Id)
        {
            pickup.SetCycle(cycle.Id);
        }


        pickup.Update(request.Data.Quantity);
        pickup.SetModified(userId);
        await _fallenStockPickupRepository.UpdateAsync(pickup, ct);

        return response;
    }
}

public class UpdateFallenStockPickupValidator : AbstractValidator<UpdateFallenStockPickupCommand>
{
    public UpdateFallenStockPickupValidator()
    {
        RuleFor(x => x.FallenStockPickupId).NotEmpty();
        RuleFor(x => x.Data.CycleId).NotEmpty();
        RuleFor(x => x.Data.Quantity)
            .GreaterThan(0)
            .WithMessage("Ilość musi być większa od zera.");
    }
}