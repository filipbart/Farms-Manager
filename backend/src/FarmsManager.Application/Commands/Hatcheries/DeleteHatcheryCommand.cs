using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Hatcheries;

public record DeleteHatcheryCommand(Guid HatcheryId) : IRequest<EmptyBaseResponse>;

public class DeleteHatcheryCommandHandler : IRequestHandler<DeleteHatcheryCommand, EmptyBaseResponse>
{
    private readonly IHatcheryRepository _hatcheryRepository;
    private readonly IUserDataResolver _userDataResolver;

    public DeleteHatcheryCommandHandler(IHatcheryRepository hatcheryRepository, IUserDataResolver userDataResolver)
    {
        _hatcheryRepository = hatcheryRepository;
        _userDataResolver = userDataResolver;
    }


    public async Task<EmptyBaseResponse> Handle(DeleteHatcheryCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var hatchery = await _hatcheryRepository.GetAsync(new HatcheryByIdSpec(request.HatcheryId), cancellationToken);
        hatchery.Delete(userId);
        await _hatcheryRepository.UpdateAsync(hatchery, cancellationToken);
        return new EmptyBaseResponse();
    }
}

public sealed class HatcheryByIdSpec : BaseSpecification<HatcheryEntity>, ISingleResultSpecification<HatcheryEntity>
{
    public HatcheryByIdSpec(Guid id)
    {
        EnsureExists();
        Query.Where(t => t.Id == id);
    }
}