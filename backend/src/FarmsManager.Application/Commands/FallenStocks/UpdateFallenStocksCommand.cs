using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.FallenStocks;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.FallenStocks;

public record UpdateFallenStockEntry
{
    public Guid HenhouseId { get; init; }
    public int Quantity { get; init; }
}

public record UpdateFallenStocksData
{
    public List<UpdateFallenStockEntry> Entries { get; init; }
}

public record UpdateFallenStocksCommand(Guid InternalGroupId, UpdateFallenStocksData Data)
    : IRequest<EmptyBaseResponse>;

public class UpdateFallenStocksCommandHandler : IRequestHandler<UpdateFallenStocksCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFallenStockRepository _fallenStockRepository;

    public UpdateFallenStocksCommandHandler(IUserDataResolver userDataResolver,
        IFallenStockRepository fallenStockRepository)
    {
        _userDataResolver = userDataResolver;
        _fallenStockRepository = fallenStockRepository;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateFallenStocksCommand request, CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var existingEntries = await _fallenStockRepository.ListAsync(
            new GetFallenStockByInternalGroupIdSpec(request.InternalGroupId), ct);

        if (existingEntries.Count == 0)
        {
            throw new Exception("Nie znaleziono żadnych wpisów sztuk upadłych o podanym identyfikatorze grupy.");
        }


        foreach (var existingEntry in existingEntries)
        {
            var updatedEntryData = request.Data.Entries
                .FirstOrDefault(e => e.HenhouseId == existingEntry.HenhouseId);

            if (updatedEntryData != null)
            {
                existingEntry.Update(updatedEntryData.Quantity);
                existingEntry.SetModified(userId);
            }
        }

        await _fallenStockRepository.UpdateRangeAsync(existingEntries, ct);

        return new EmptyBaseResponse();
    }
}

public class UpdateFallenStocksValidator : AbstractValidator<UpdateFallenStocksCommand>
{
    public UpdateFallenStocksValidator()
    {
        RuleFor(x => x.InternalGroupId).NotEmpty();
        RuleFor(x => x.Data).NotNull();
        RuleFor(x => x.Data.Entries).NotEmpty()
            .WithMessage("Należy dostarczyć co najmniej jedną pozycję do aktualizacji.");

        RuleForEach(x => x.Data.Entries).ChildRules(entry =>
        {
            entry.RuleFor(e => e.HenhouseId).NotEmpty();
            entry.RuleFor(e => e.Quantity).GreaterThan(0);
        });
    }
}