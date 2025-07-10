using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Feeds;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Feeds.Prices;

public record DeleteFeedPriceCommand(Guid Id) : IRequest<EmptyBaseResponse>;

public class DeleteFeedPriceCommandHandler : IRequestHandler<DeleteFeedPriceCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFeedPriceRepository _feedPriceRepository;

    public DeleteFeedPriceCommandHandler(IUserDataResolver userDataResolver, IFeedPriceRepository feedPriceRepository)
    {
        _userDataResolver = userDataResolver;
        _feedPriceRepository = feedPriceRepository;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteFeedPriceCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var feedPrice = await _feedPriceRepository.GetAsync(new FeedPriceByIdSpec(request.Id),
            cancellationToken);

        feedPrice.Delete(userId);
        await _feedPriceRepository.UpdateAsync(feedPrice, cancellationToken);

        return new EmptyBaseResponse();
    }
}