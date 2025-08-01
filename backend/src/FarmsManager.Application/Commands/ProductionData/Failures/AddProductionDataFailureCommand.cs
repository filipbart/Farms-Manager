using Ardalis.Specification;
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

public record ProductionDataFailureEntryDto
{
    public Guid HenhouseId { get; init; }
    public int DeadCount { get; init; }
    public int DefectiveCount { get; init; }
}

public record AddProductionDataFailureCommand : IRequest<EmptyBaseResponse>
{
    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public List<ProductionDataFailureEntryDto> FailureEntries { get; init; } = [];
}

public class
    AddProductionDataFailureCommandHandler : IRequestHandler<AddProductionDataFailureCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IProductionDataFailureRepository _failureRepository;

    public AddProductionDataFailureCommandHandler(
        IUserDataResolver userDataResolver,
        IFarmRepository farmRepository,
        ICycleRepository cycleRepository,
        IProductionDataFailureRepository failureRepository)
    {
        _userDataResolver = userDataResolver;
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
        _failureRepository = failureRepository;
    }

    public async Task<EmptyBaseResponse> Handle(AddProductionDataFailureCommand request, CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.FarmId), ct);
        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(request.CycleId), ct);

        var henhouseIds = request.FailureEntries.Select(e => e.HenhouseId).ToList();
        var existingFailures = await _failureRepository.ListAsync(
            new GetFailuresByHenhousesSpec(farm.Id, cycle.Id, henhouseIds), ct);

        if (existingFailures.Count != 0)
        {
            var existingHenhouseNames = existingFailures.Select(f => f.Henhouse.Name).Distinct();
            throw new Exception(
                $"Wpisy dla kurników: {string.Join(", ", existingHenhouseNames)} już istnieją w tym cyklu.");
        }

        var newFailures = request.FailureEntries
            .Select(entry => ProductionDataFailureEntity.CreateNew(
                farm.Id,
                cycle.Id,
                entry.HenhouseId,
                entry.DeadCount,
                entry.DefectiveCount,
                userId))
            .ToList();

        await _failureRepository.AddRangeAsync(newFailures, ct);

        return BaseResponse.EmptyResponse;
    }
}

public class AddProductionDataFailureValidator : AbstractValidator<AddProductionDataFailureCommand>
{
    public AddProductionDataFailureValidator()
    {
        RuleFor(t => t.FarmId).NotEmpty();
        RuleFor(t => t.CycleId).NotEmpty();
        RuleFor(t => t.FailureEntries).NotEmpty().WithMessage("Należy dodać przynajmniej jeden wpis dla kurnika.");

        RuleForEach(t => t.FailureEntries).ChildRules(entry =>
        {
            entry.RuleFor(e => e.HenhouseId).NotEmpty();
            entry.RuleFor(e => e.DeadCount).GreaterThanOrEqualTo(0);
            entry.RuleFor(e => e.DefectiveCount).GreaterThanOrEqualTo(0);
        });
    }
}

public sealed class GetFailuresByHenhousesSpec : BaseSpecification<ProductionDataFailureEntity>
{
    public GetFailuresByHenhousesSpec(Guid farmId, Guid cycleId, List<Guid> henhouseIds)
    {
        EnsureExists();
        Query.Where(t => t.FarmId == farmId
                         && t.CycleId == cycleId
                         && henhouseIds.Contains(t.HenhouseId));

        Query.Include(t => t.Henhouse);
    }
}