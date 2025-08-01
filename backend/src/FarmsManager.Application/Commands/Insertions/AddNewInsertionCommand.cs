﻿using Ardalis.Specification;
using FarmsManager.Application.Commands.Farms;
using FarmsManager.Application.Commands.Hatcheries;
using FarmsManager.Application.Commands.ProductionData.Weighings;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Insertions;

public record AddNewInsertionCommand : IRequest<BaseResponse<AddNewInsertionCommandResponse>>
{
    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public DateOnly InsertionDate { get; init; }
    public List<InsertionEntry> Entries { get; init; } = [];

    /// <summary>
    /// Entries
    /// </summary>
    public record InsertionEntry
    {
        public Guid HenhouseId { get; init; }
        public Guid HatcheryId { get; init; }
        public int Quantity { get; init; }
        public decimal BodyWeight { get; init; }
    }
}

public class AddNewInsertionCommandResponse
{
    public Guid InternalGroupId { get; set; }
}

public class
    AddNewInsertionCommandHandler : IRequestHandler<AddNewInsertionCommand,
    BaseResponse<AddNewInsertionCommandResponse>>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IInsertionRepository _insertionRepository;
    private readonly IHenhouseRepository _henhouseRepository;
    private readonly IHatcheryRepository _hatcheryRepository;
    private readonly IProductionDataWeighingRepository _productionDataWeighingRepository;

    public AddNewInsertionCommandHandler(IUserDataResolver userDataResolver, IInsertionRepository insertionRepository,
        IHenhouseRepository henhouseRepository, IFarmRepository farmRepository, ICycleRepository cycleRepository,
        IHatcheryRepository hatcheryRepository, IProductionDataWeighingRepository productionDataWeighingRepository)
    {
        _userDataResolver = userDataResolver;
        _insertionRepository = insertionRepository;
        _henhouseRepository = henhouseRepository;
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
        _hatcheryRepository = hatcheryRepository;
        _productionDataWeighingRepository = productionDataWeighingRepository;
    }

    public async Task<BaseResponse<AddNewInsertionCommandResponse>> Handle(AddNewInsertionCommand request,
        CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var response = new BaseResponse<AddNewInsertionCommandResponse>
        {
            ResponseData = new AddNewInsertionCommandResponse()
        };

        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.FarmId), ct);
        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(request.CycleId), ct);

        var duplicateHenhouseIds = request.Entries
            .GroupBy(entry => entry.HenhouseId)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        if (duplicateHenhouseIds.Count != 0)
        {
            response.AddError("Henhouses", "W podanych danych wejściowych powtarzają się dane dla tego samego kurnika");
        }

        var existedInsertions = await _insertionRepository.ListAsync(
            new GetInsertionByFarmCycleAndHenhouseIdSpec(request.FarmId, request.CycleId,
                request.Entries.Select(t => t.HenhouseId).ToArray()),
            ct);

        if (existedInsertions.Count != 0)
        {
            foreach (var existedInsertion in existedInsertions)
            {
                var henhouse = await _henhouseRepository.GetAsync(new HenhouseByIdSpec(existedInsertion.HenhouseId),
                    ct);

                throw DomainException.InsertionExists(henhouse.Name);
            }
        }

        var newInsertions = new List<InsertionEntity>();
        var internalGroupId = Guid.NewGuid();
        foreach (var entry in request.Entries)
        {
            var henhouse = await _henhouseRepository.GetAsync(new HenhouseByIdSpec(entry.HenhouseId),
                ct);
            var hatchery =
                await _hatcheryRepository.GetAsync(new HatcheryByIdSpec(entry.HatcheryId), ct);

            var newInsertion = InsertionEntity.CreateNew(internalGroupId, farm.Id, cycle.Id, henhouse.Id, hatchery.Id,
                request.InsertionDate, entry.Quantity, entry.BodyWeight, userId);
            newInsertions.Add(newInsertion);
            var existingWeighing = await _productionDataWeighingRepository.FirstOrDefaultAsync(
                new GetWeighingByKeysSpec(farm.Id, cycle.Id, henhouse.Id, hatchery.Id), ct);

            if (existingWeighing is null)
            {
                var newWeighing = ProductionDataWeighingEntity.CreateNew(
                    farm.Id,
                    henhouse.Id,
                    cycle.Id,
                    hatchery.Id,
                    0,
                    entry.BodyWeight,
                    userId
                );

                await _productionDataWeighingRepository.AddAsync(newWeighing, ct);
            }
        }

        if (newInsertions.Count != 0)
            await _insertionRepository.AddRangeAsync(newInsertions, ct);

        response.ResponseData.InternalGroupId = internalGroupId;

        return response;
    }
}

public class AddNewInsertionValidator : AbstractValidator<AddNewInsertionCommand>
{
    public AddNewInsertionValidator()
    {
        RuleFor(t => t.InsertionDate).LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now));
        RuleFor(t => t.Entries).NotEmpty();
        RuleForEach(t => t.Entries).ChildRules(t =>
        {
            t.RuleFor(x => x.Quantity).GreaterThan(0);
            t.RuleFor(x => x.BodyWeight).GreaterThan(0);
        });
    }
}

public sealed class GetInsertionByFarmCycleAndHenhouseIdSpec : BaseSpecification<InsertionEntity>
{
    public GetInsertionByFarmCycleAndHenhouseIdSpec(Guid farmId, Guid cycleId, Guid[] henhouseIds)
    {
        EnsureExists();
        Query.Where(t => t.FarmId == farmId);
        Query.Where(t => t.CycleId == cycleId);
        Query.Where(t => henhouseIds.Contains(t.HenhouseId));
    }
}