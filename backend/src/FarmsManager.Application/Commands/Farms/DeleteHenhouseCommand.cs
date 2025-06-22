using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Farms;

public record DeleteHenhouseCommand(Guid HenhouseId) : IRequest<EmptyBaseResponse>;

public class DeleteHenhouseCommandHandler : IRequestHandler<DeleteHenhouseCommand, EmptyBaseResponse>
{
    private readonly IHenhouseRepository _henhouseRepository;
    private readonly IUserDataResolver _userDataResolver;

    public DeleteHenhouseCommandHandler(IHenhouseRepository henhouseRepository, IUserDataResolver userDataResolver)
    {
        _henhouseRepository = henhouseRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteHenhouseCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var henhouse = await _henhouseRepository.GetAsync(new HenhouseByIdSpec(request.HenhouseId), cancellationToken);
        henhouse.Delete(userId);
        await _henhouseRepository.UpdateAsync(henhouse, cancellationToken);
        return new EmptyBaseResponse();
    }
}

public sealed class HenhouseByIdSpec : BaseSpecification<HenhouseEntity>, ISingleResultSpecification<HenhouseEntity>
{
    public HenhouseByIdSpec(Guid id)
    {
        EnsureExists();
        Query.Where(t => t.Id == id);
    }
}