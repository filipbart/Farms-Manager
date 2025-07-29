using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.ProductionData;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.ProductionData.RemainingFeed;

public record DeleteProductionDataRemainingFeedCommand(Guid Id) : IRequest<EmptyBaseResponse>;

public class
    DeleteProductionDataRemainingFeedCommandHandler : IRequestHandler<DeleteProductionDataRemainingFeedCommand,
    EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IProductionDataRemainingFeedRepository _repository;

    public DeleteProductionDataRemainingFeedCommandHandler(IUserDataResolver userDataResolver,
        IProductionDataRemainingFeedRepository repository)
    {
        _userDataResolver = userDataResolver;
        _repository = repository;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteProductionDataRemainingFeedCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var remainingFeed =
            await _repository.GetAsync(new ProductionDataRemainingFeedByIdSpec(request.Id),
                cancellationToken);

        remainingFeed.Delete(userId);
        await _repository.UpdateAsync(remainingFeed, cancellationToken);

        return new EmptyBaseResponse();
    }
}