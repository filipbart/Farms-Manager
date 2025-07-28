using Ardalis.Specification;
using FarmsManager.Application.Commands.Farms;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;


namespace FarmsManager.Application.Commands.ProductionData.Failures;

public record AddProductionDataFailureCommand : IRequest<EmptyBaseResponse>
{
    public Guid FarmId { get; init; }
    public Guid HenhouseId { get; init; }
    public Guid CycleId { get; init; }
    public int DeadCount { get; init; }
    public int DefectiveCount { get; init; }
}

public class AddProductionDataFailureCommandHandler : IRequestHandler<AddProductionDataFailureCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IHenhouseRepository _henhouseRepository;
    private readonly IProductionDataFailureRepository _failureRepository;

    public AddProductionDataFailureCommandHandler(
        IUserDataResolver userDataResolver,
        IFarmRepository farmRepository,
        ICycleRepository cycleRepository,
        IHenhouseRepository henhouseRepository,
        IProductionDataFailureRepository failureRepository)
    {
        _userDataResolver = userDataResolver;
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
        _henhouseRepository = henhouseRepository;
        _failureRepository = failureRepository;
    }

    public async Task<EmptyBaseResponse> Handle(AddProductionDataFailureCommand request, CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.FarmId), ct);
        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(request.CycleId), ct);
        var henhouse = await _henhouseRepository.GetAsync(new HenhouseByIdSpec(request.HenhouseId), ct);

        var existingFailure = await _failureRepository.FirstOrDefaultAsync(
            new GetFailureByFarmCycleAndHenhouseIdSpec(farm.Id, cycle.Id, henhouse.Id), ct);

        if (existingFailure is not null)
        {
            throw new Exception(
                $"Wpis o upadkach/wybrakowaniach dla kurnika '{henhouse.Name}' w tym cyklu już istnieje.");
        }

        var newFailure = ProductionDataFailureEntity.CreateNew(
            farm.Id,
            cycle.Id,
            henhouse.Id,
            request.DeadCount,
            request.DefectiveCount,
            userId
        );

        await _failureRepository.AddAsync(newFailure, ct);

        return BaseResponse.EmptyResponse;
    }
}

public class AddProductionDataFailureValidator : AbstractValidator<AddProductionDataFailureCommand>
{
    public AddProductionDataFailureValidator()
    {
        RuleFor(t => t.FarmId).NotEmpty();
        RuleFor(t => t.CycleId).NotEmpty();
        RuleFor(t => t.HenhouseId).NotEmpty();
        RuleFor(t => t.DeadCount).GreaterThanOrEqualTo(0);
        RuleFor(t => t.DefectiveCount).GreaterThanOrEqualTo(0);
    }
}

public sealed class GetFailureByFarmCycleAndHenhouseIdSpec : BaseSpecification<ProductionDataFailureEntity>
{
    public GetFailureByFarmCycleAndHenhouseIdSpec(Guid farmId, Guid cycleId, Guid henhouseId)
    {
        EnsureExists();
        Query.Where(t => t.FarmId == farmId && t.CycleId == cycleId && t.HenhouseId == henhouseId);
    }
}