using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.ProductionData;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.ProductionData.TransferFeed;

public record DeleteProductionDataTransferFeedCommand(Guid Id) : IRequest<EmptyBaseResponse>;

public class
    DeleteProductionDataTransferFeedCommandHandler : IRequestHandler<DeleteProductionDataTransferFeedCommand,
    EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IProductionDataTransferFeedRepository _repository;

    public DeleteProductionDataTransferFeedCommandHandler(IUserDataResolver userDataResolver,
        IProductionDataTransferFeedRepository repository)
    {
        _userDataResolver = userDataResolver;
        _repository = repository;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteProductionDataTransferFeedCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var transferFeed =
            await _repository.GetAsync(new ProductionDataTransferFeedByIdSpec(request.Id),
                cancellationToken);

        transferFeed.Delete(userId);
        await _repository.UpdateAsync(transferFeed, cancellationToken);

        return new EmptyBaseResponse();
    }
}