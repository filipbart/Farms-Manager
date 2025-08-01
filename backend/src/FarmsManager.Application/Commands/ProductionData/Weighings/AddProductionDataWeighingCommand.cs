using Ardalis.Specification;
using FarmsManager.Application.Commands.Farms;
using FarmsManager.Application.Commands.Hatcheries;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.ProductionData.Weighings;

public record AddProductionDataWeighingCommand : IRequest<EmptyBaseResponse>
{
    public Guid FarmId { get; init; }
    public Guid HenhouseId { get; init; }
    public Guid CycleId { get; init; }
    public Guid HatcheryId { get; init; }
    public int Day { get; init; }
    public decimal Weight { get; init; }
}

public class
    AddProductionDataWeighingCommandHandler : IRequestHandler<AddProductionDataWeighingCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IHenhouseRepository _henhouseRepository;
    private readonly IHatcheryRepository _hatcheryRepository;
    private readonly IProductionDataWeighingRepository _weighingRepository;

    public AddProductionDataWeighingCommandHandler(
        IUserDataResolver userDataResolver,
        IFarmRepository farmRepository,
        ICycleRepository cycleRepository,
        IHenhouseRepository henhouseRepository,
        IHatcheryRepository hatcheryRepository,
        IProductionDataWeighingRepository weighingRepository)
    {
        _userDataResolver = userDataResolver;
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
        _henhouseRepository = henhouseRepository;
        _hatcheryRepository = hatcheryRepository;
        _weighingRepository = weighingRepository;
    }

    public async Task<EmptyBaseResponse> Handle(AddProductionDataWeighingCommand request, CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();


        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.FarmId), ct);
        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(request.CycleId), ct);
        var henhouse = await _henhouseRepository.GetAsync(new HenhouseByIdSpec(request.HenhouseId), ct);
        var hatchery = await _hatcheryRepository.GetAsync(new HatcheryByIdSpec(request.HatcheryId), ct);


        var existingWeighing = await _weighingRepository.FirstOrDefaultAsync(
            new GetWeighingByKeysSpec(farm.Id, cycle.Id, henhouse.Id, hatchery.Id), ct);

        if (existingWeighing is not null)
        {
            throw new Exception(
                $"Rekord ważenia dla kurnika '{henhouse.Name}' oraz dla wylęgarni '{hatchery.Name}' w tym cyklu oraz dla tej farmy już istnieje. Użyj opcji edycji, aby dodać kolejne ważenia.");
        }

        var newWeighing = ProductionDataWeighingEntity.CreateNew(
            farm.Id,
            henhouse.Id,
            cycle.Id,
            hatchery.Id,
            request.Day,
            request.Weight,
            userId
        );

        await _weighingRepository.AddAsync(newWeighing, ct);

        return BaseResponse.EmptyResponse;
    }
}

public class AddProductionDataWeighingValidator : AbstractValidator<AddProductionDataWeighingCommand>
{
    public AddProductionDataWeighingValidator()
    {
        RuleFor(t => t.FarmId).NotEmpty();
        RuleFor(t => t.CycleId).NotEmpty();
        RuleFor(t => t.HenhouseId).NotEmpty();
        RuleFor(t => t.Day).GreaterThanOrEqualTo(0);
        RuleFor(t => t.Weight).GreaterThan(0);
    }
}

public sealed class GetWeighingByKeysSpec : BaseSpecification<ProductionDataWeighingEntity>
{
    public GetWeighingByKeysSpec(Guid farmId, Guid cycleId, Guid henhouseId, Guid hatcheryId)
    {
        EnsureExists();

        Query.Where(t =>
            t.FarmId == farmId && t.CycleId == cycleId && t.HenhouseId == henhouseId && t.HatcheryId == hatcheryId);
    }
}