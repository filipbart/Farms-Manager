using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Entities;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.FallenStockPickups;

public record DeleteFallenStockPickupCommand(Guid FallenStockPickupId) : IRequest<EmptyBaseResponse>;

public class DeleteFallenStockPickupCommandHandler
    : IRequestHandler<DeleteFallenStockPickupCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFallenStockPickupRepository _fallenStockPickupRepository;

    public DeleteFallenStockPickupCommandHandler(
        IUserDataResolver userDataResolver,
        IFallenStockPickupRepository fallenStockPickupRepository)
    {
        _userDataResolver = userDataResolver;
        _fallenStockPickupRepository = fallenStockPickupRepository;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteFallenStockPickupCommand request, CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var response = new EmptyBaseResponse();

        var pickup =
            await _fallenStockPickupRepository.GetAsync(new FallenStockPickupByIdSpec(request.FallenStockPickupId), ct);


        pickup.Delete(userId);
        await _fallenStockPickupRepository.UpdateAsync(pickup, ct);

        return response;
    }
}

public sealed class FallenStockPickupByIdSpec : BaseSpecification<FallenStockPickupEntity>,
    ISingleResultSpecification<FallenStockPickupEntity>
{
    public FallenStockPickupByIdSpec(Guid id)
    {
        EnsureExists();
        Query.Where(t => t.Id == id);
    }
}