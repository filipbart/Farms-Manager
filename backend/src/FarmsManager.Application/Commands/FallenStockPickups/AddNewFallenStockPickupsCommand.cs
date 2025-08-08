using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Entities;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.FallenStockPickups;

public record AddNewFallenStockPickupsCommand : IRequest<EmptyBaseResponse>
{
    public Guid FarmId { get; set; }
    public Guid CycleId { get; set; }
    public List<FallenStockPickupEntryDto> Entries { get; set; } = [];
}

public record FallenStockPickupEntryDto
{
    public DateOnly Date { get; set; }
    public int Quantity { get; set; }
}

public class AddNewFallenStockPickupsCommandHandler
    : IRequestHandler<AddNewFallenStockPickupsCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IFallenStockPickupRepository _fallenStockPickupRepository;

    public AddNewFallenStockPickupsCommandHandler(
        IUserDataResolver userDataResolver,
        IFarmRepository farmRepository,
        ICycleRepository cycleRepository,
        IFallenStockPickupRepository fallenStockPickupRepository)
    {
        _userDataResolver = userDataResolver;
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
        _fallenStockPickupRepository = fallenStockPickupRepository;
    }

    public async Task<EmptyBaseResponse> Handle(AddNewFallenStockPickupsCommand request, CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var response = new EmptyBaseResponse();

        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.FarmId), ct);
        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(request.CycleId), ct);

        var duplicateDates = request.Entries
            .GroupBy(e => e.Date)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateDates.Count > 0)
        {
            response.AddError("Entries", "Wprowadzono zduplikowane wpisy dla tej samej daty.");
            return response;
        }

        foreach (var entry in request.Entries)
        {
            var existing = await _fallenStockPickupRepository.FirstOrDefaultAsync(
                new GetFallenStockPickupByDateSpec(request.FarmId, request.CycleId, entry.Date), ct);

            if (existing != null)
            {
                response.AddError("Entries", $"Dla daty {entry.Date} pickup już istnieje.");
                return response;
            }
        }

        var newPickups = request.Entries.Select(entry =>
            FallenStockPickupEntity.CreateNew(
                farm.Id,
                cycle.Id,
                entry.Date,
                entry.Quantity,
                userId
            )).ToList();

        if (newPickups.Count > 0)
            await _fallenStockPickupRepository.AddRangeAsync(newPickups, ct);

        return response;
    }
}

public class AddNewFallenStockPickupsValidator : AbstractValidator<AddNewFallenStockPickupsCommand>
{
    public AddNewFallenStockPickupsValidator()
    {
        RuleFor(x => x.FarmId).NotEmpty();
        RuleFor(x => x.CycleId).NotEmpty();
        RuleFor(x => x.Entries).NotEmpty()
            .WithMessage("Należy dodać co najmniej jedną pozycję.");

        RuleForEach(x => x.Entries).ChildRules(entry =>
        {
            entry.RuleFor(e => e.Date).NotEmpty();
            entry.RuleFor(e => e.Quantity).GreaterThan(0);
        });
    }
}

public sealed class GetFallenStockPickupByDateSpec
    : BaseSpecification<FallenStockPickupEntity>, ISingleResultSpecification<FallenStockPickupEntity>
{
    public GetFallenStockPickupByDateSpec(Guid farmId, Guid cycleId, DateOnly date)
    {
        EnsureExists();
        DisableTracking();
        Query.Where(fs => fs.FarmId == farmId && fs.CycleId == cycleId && fs.Date == date)
            .Take(1);
    }
}