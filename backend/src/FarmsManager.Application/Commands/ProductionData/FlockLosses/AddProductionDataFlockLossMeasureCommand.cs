using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.ProductionData.FlockLosses;

public record FlockLossMeasureEntryDto
{
    public Guid HenhouseId { get; init; }
    public int Quantity { get; init; }
}

public record AddProductionDataFlockLossCommand : IRequest<EmptyBaseResponse>
{
    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public int MeasureNumber { get; init; }
    public int Day { get; init; }
    public List<FlockLossMeasureEntryDto> Entries { get; init; } = [];
}

public class
    AddProductionDataFlockLossCommandHandler : IRequestHandler<AddProductionDataFlockLossCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IProductionDataFlockLossMeasureRepository _flockLossRepository;
    private readonly IInsertionRepository _insertionRepository;

    public AddProductionDataFlockLossCommandHandler(
        IUserDataResolver userDataResolver,
        IProductionDataFlockLossMeasureRepository flockLossRepository,
        IInsertionRepository insertionRepository)
    {
        _userDataResolver = userDataResolver;
        _flockLossRepository = flockLossRepository;
        _insertionRepository = insertionRepository;
    }

    public async Task<EmptyBaseResponse> Handle(AddProductionDataFlockLossCommand request, CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var updatedEntities = new List<ProductionDataFlockLossMeasureEntity>();
        var newEntities = new List<ProductionDataFlockLossMeasureEntity>();

        foreach (var entry in request.Entries)
        {
            var spec = new GetFlockLossMeasureByKeysSpec(request.FarmId, request.CycleId, entry.HenhouseId);
            var flockLossMeasure = await _flockLossRepository.FirstOrDefaultAsync(spec, ct);

            if (flockLossMeasure is not null)
            {
                var percentage = flockLossMeasure.Insertion.Quantity > 0
                    ? (decimal)entry.Quantity / flockLossMeasure.Insertion.Quantity * 100
                    : 0;

                flockLossMeasure.UpdateMeasure(request.MeasureNumber, request.Day, entry.Quantity);
                flockLossMeasure.SetModified(userId);
                updatedEntities.Add(flockLossMeasure);
            }
            else
            {
                var insertionSpec = new GetInsertionByKeysSpec(request.FarmId, request.CycleId, entry.HenhouseId);
                var insertion = await _insertionRepository.FirstOrDefaultAsync(insertionSpec, ct);

                if (insertion is null)
                {
                    throw new Exception($"Nie znaleziono aktywnego wstawienia dla kurnika o ID: {entry.HenhouseId}.");
                }

                var newEntity = ProductionDataFlockLossMeasureEntity.CreateNew(
                    request.FarmId,
                    entry.HenhouseId,
                    request.CycleId,
                    insertion.HatcheryId,
                    insertion.Id,
                    request.Day,
                    entry.Quantity,
                    userId
                );
                newEntities.Add(newEntity);
            }
        }

        if (updatedEntities.Count != 0)
        {
            await _flockLossRepository.UpdateRangeAsync(updatedEntities, ct);
        }

        if (newEntities.Count != 0)
        {
            await _flockLossRepository.AddRangeAsync(newEntities, ct);
        }

        return BaseResponse.EmptyResponse;
    }
}

public class AddProductionDataFlockLossValidator : AbstractValidator<AddProductionDataFlockLossCommand>
{
    public AddProductionDataFlockLossValidator()
    {
        RuleFor(t => t.FarmId).NotEmpty();
        RuleFor(t => t.CycleId).NotEmpty();
        RuleFor(t => t.Day).GreaterThan(0);
        RuleFor(x => x.MeasureNumber).InclusiveBetween(1, 4).WithMessage("Numer pomiaru musi być od 1 do 4.");
        RuleFor(t => t.Entries).NotEmpty();

        RuleForEach(t => t.Entries).ChildRules(entry =>
        {
            entry.RuleFor(e => e.HenhouseId).NotEmpty();
            entry.RuleFor(e => e.Quantity).GreaterThan(0);
        });
    }
}

public sealed class GetFlockLossMeasureByKeysSpec : BaseSpecification<ProductionDataFlockLossMeasureEntity>
{
    public GetFlockLossMeasureByKeysSpec(Guid farmId, Guid cycleId, Guid henhouseId)
    {
        EnsureExists();
        Query
            .Include(e => e.Insertion)
            .Where(t =>
                t.FarmId == farmId && t.CycleId == cycleId && t.HenhouseId == henhouseId);
    }
}

public sealed class GetInsertionByKeysSpec : BaseSpecification<InsertionEntity>
{
    public GetInsertionByKeysSpec(Guid farmId, Guid cycleId, Guid henhouseId)
    {
        EnsureExists();
        DisableTracking();
        Query.Where(t => t.FarmId == farmId);
        Query.Where(t => t.CycleId == cycleId);
        Query.Where(t => t.HenhouseId == henhouseId);
        Query.OrderByDescending(t => t.InsertionDate);
        Query.Take(1);
    }
}