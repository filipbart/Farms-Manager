using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.FallenStocks;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.FallenStocks;

public record DeleteFallenStocksCommand(Guid InternalGroupId) : IRequest<EmptyBaseResponse>;

public class DeleteFallenStocksCommandHandler : IRequestHandler<DeleteFallenStocksCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFallenStockRepository _fallenStockRepository;

    public DeleteFallenStocksCommandHandler(IUserDataResolver userDataResolver,
        IFallenStockRepository fallenStockRepository)
    {
        _userDataResolver = userDataResolver;
        _fallenStockRepository = fallenStockRepository;
    }


    public async Task<EmptyBaseResponse> Handle(DeleteFallenStocksCommand request, CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var response = new EmptyBaseResponse();

        var fallenStocksToDelete = await _fallenStockRepository.ListAsync(
            new GetFallenStockByInternalGroupIdSpec(request.InternalGroupId), ct);

        if (fallenStocksToDelete.Count == 0)
        {
            return response;
        }

        if (fallenStocksToDelete.Any(fs => fs.DateIrzSentUtc.HasValue))
        {
            response.AddError("Delete", "Nie można usunąć zgłoszenia, ponieważ zostało już wysłane do IRZplus.");
            return response;
        }

        foreach (var fallenStockEntity in fallenStocksToDelete)
        {
            fallenStockEntity.Delete(userId);
        }

        await _fallenStockRepository.UpdateRangeAsync(fallenStocksToDelete, ct);
        return response;
    }
}

public class DeleteFallenStocksValidator : AbstractValidator<DeleteFallenStocksCommand>
{
    public DeleteFallenStocksValidator()
    {
        RuleFor(x => x.InternalGroupId).NotEmpty();
    }
}