using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.SlaughterhouseAggregate.Entities;
using FarmsManager.Domain.Aggregates.SlaughterhouseAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Slaughterhouses;

public record DeleteSlaughterhouseCommand(Guid SlaughterhouseId) : IRequest<EmptyBaseResponse>;

public class DeleteSlaughterhouseCommandHandler : IRequestHandler<DeleteSlaughterhouseCommand, EmptyBaseResponse>
{
    private readonly ISlaughterhouseRepository _slaughterhouseRepository;
    private readonly IUserDataResolver _userDataResolver;

    public DeleteSlaughterhouseCommandHandler(ISlaughterhouseRepository slaughterhouseRepository,
        IUserDataResolver userDataResolver)
    {
        _slaughterhouseRepository = slaughterhouseRepository;
        _userDataResolver = userDataResolver;
    }


    public async Task<EmptyBaseResponse> Handle(DeleteSlaughterhouseCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var slaughterhouse =
            await _slaughterhouseRepository.GetAsync(new SlaughterhouseByIdSpec(request.SlaughterhouseId),
                cancellationToken);
        slaughterhouse.Delete(userId);
        await _slaughterhouseRepository.UpdateAsync(slaughterhouse, cancellationToken);
        return new EmptyBaseResponse();
    }
}

public sealed class SlaughterhouseByIdSpec : BaseSpecification<SlaughterhouseEntity>,
    ISingleResultSpecification<SlaughterhouseEntity>
{
    public SlaughterhouseByIdSpec(Guid id)
    {
        EnsureExists();
        Query.Where(t => t.Id == id);
    }
}