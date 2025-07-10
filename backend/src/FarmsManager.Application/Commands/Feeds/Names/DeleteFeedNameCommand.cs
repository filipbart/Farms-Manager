using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entites;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Feeds.Names;

public record DeleteFeedNameCommand(Guid FeedNameId) : IRequest<EmptyBaseResponse>;

public class DeleteFeedNameCommandHandler : IRequestHandler<DeleteFeedNameCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFeedNameRepository _feedNameRepository;

    public DeleteFeedNameCommandHandler(
        IUserDataResolver userDataResolver,
        IFeedNameRepository feedNameRepository)
    {
        _userDataResolver = userDataResolver;
        _feedNameRepository = feedNameRepository;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteFeedNameCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var entity = await _feedNameRepository.GetAsync(new GetFeedNameByIdSpec(request.FeedNameId),
            cancellationToken);

        entity.Delete(userId);
        await _feedNameRepository.UpdateAsync(entity, cancellationToken);

        return new EmptyBaseResponse();
    }
}

public sealed class GetFeedNameByIdSpec : BaseSpecification<FeedNameEntity>,
    ISingleResultSpecification<FeedNameEntity>
{
    public GetFeedNameByIdSpec(Guid id)
    {
        EnsureExists();
        Query.Where(t => t.Id == id);
    }
}