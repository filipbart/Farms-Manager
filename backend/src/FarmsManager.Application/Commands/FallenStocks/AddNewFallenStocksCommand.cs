using Ardalis.Specification;
using FarmsManager.Application.Commands.Farms;
using FarmsManager.Application.Commands.UtilizationPlants;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Entities;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.FallenStocks;

public record AddNewFallenStocksCommand : IRequest<EmptyBaseResponse>
{
    public Guid FarmId { get; set; }
    public Guid CycleId { get; set; }
    public Guid UtilizationPlantId { get; set; }
    public DateOnly Date { get; set; }
    public List<FallenStockEntryDto> Entries { get; set; } = [];
    public bool SendToIrz { get; set; }
}

public record FallenStockEntryDto
{
    public Guid HenhouseId { get; set; }
    public int Quantity { get; set; }
}

public class AddNewFallenStocksCommandHandler : IRequestHandler<AddNewFallenStocksCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IFallenStockRepository _fallenStockRepository;
    private readonly IHenhouseRepository _henhouseRepository;
    private readonly IUtilizationPlantRepository _utilizationPlantRepository;
    private readonly IIrzplusService _irzplusService;


    public AddNewFallenStocksCommandHandler(IUserDataResolver userDataResolver, IUserRepository userRepository,
        IFarmRepository farmRepository, ICycleRepository cycleRepository, IFallenStockRepository fallenStockRepository,
        IHenhouseRepository henhouseRepository, IUtilizationPlantRepository utilizationPlantRepository,
        IIrzplusService irzplusService)
    {
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
        _fallenStockRepository = fallenStockRepository;
        _henhouseRepository = henhouseRepository;
        _utilizationPlantRepository = utilizationPlantRepository;
        _irzplusService = irzplusService;
    }

    public async Task<EmptyBaseResponse> Handle(AddNewFallenStocksCommand request, CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.SingleOrDefaultAsync(new UserByIdSpec(userId), ct) ??
                   throw DomainException.UserNotFound();
        var response = new EmptyBaseResponse();

        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.FarmId), ct);
        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(request.CycleId), ct);
        var utilizationPlant =
            await _utilizationPlantRepository.GetAsync(new UtilizationPlantByIdSpec(request.UtilizationPlantId), ct);

        var duplicateHenhouseIds = request.Entries
            .GroupBy(entry => entry.HenhouseId)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        if (duplicateHenhouseIds.Count != 0)
        {
            response.AddError("Entries", "Wprowadzono zduplikowane wpisy dla tego samego kurnika.");
            return response;
        }

        var henhouseIds = request.Entries.Select(e => e.HenhouseId).ToArray();
        var existingFallenStockOnDate = await _fallenStockRepository.ListAsync(
            new GetFallenStockByDateAndHenhouseIdsSpec(request.Date, henhouseIds), ct);

        if (existingFallenStockOnDate.Count != 0)
        {
            var existingHenhouseId = existingFallenStockOnDate.First().HenhouseId;
            var henhouse = await _henhouseRepository.GetByIdAsync(existingHenhouseId, ct);
            response.AddError("Entries", $"Kurnik '{henhouse?.Name}' ma już zgłoszenie w tym dniu.");
            return response;
        }

        Guid internalGroupId;
        var existingGroupForDate = await _fallenStockRepository.FirstOrDefaultAsync(
            new GetFallenStockByDateSpec(request.FarmId, request.CycleId, request.Date), ct);

        if (existingGroupForDate != null)
        {
            internalGroupId = existingGroupForDate.InternalGroupId;
        }
        else
        {
            internalGroupId = Guid.NewGuid();
        }

        var newFallenStocks = new List<FallenStockEntity>();
        foreach (var entry in request.Entries)
        {
            var henhouse = await _henhouseRepository.GetAsync(new HenhouseByIdSpec(entry.HenhouseId), ct);

            var newFallenStock = FallenStockEntity.CreateNew(
                internalGroupId,
                farm.Id,
                cycle.Id,
                utilizationPlant.Id,
                henhouse.Id,
                request.Date,
                entry.Quantity,
                userId
            );
            newFallenStocks.Add(newFallenStock);
        }

        if (newFallenStocks.Count != 0)
        {
            await _fallenStockRepository.AddRangeAsync(newFallenStocks, ct);
            if (request.SendToIrz)
            {
                //TODO _irzplusService.PrepareOptions(user.IrzplusCredentials);
            }
        }

        return response;
    }
}

public class AddNewFallenStocksValidator : AbstractValidator<AddNewFallenStocksCommand>
{
    public AddNewFallenStocksValidator()
    {
        RuleFor(x => x.FarmId).NotEmpty();
        RuleFor(x => x.CycleId).NotEmpty();
        RuleFor(x => x.UtilizationPlantId).NotEmpty();
        RuleFor(x => x.Date).NotEmpty();
        RuleFor(x => x.Entries).NotEmpty().WithMessage("Należy dodać co najmniej jedną pozycję.");

        RuleForEach(x => x.Entries).ChildRules(entry =>
        {
            entry.RuleFor(e => e.HenhouseId).NotEmpty();
            entry.RuleFor(e => e.Quantity).GreaterThan(0);
        });
    }
}

public sealed class GetFallenStockByDateAndHenhouseIdsSpec : BaseSpecification<FallenStockEntity>
{
    public GetFallenStockByDateAndHenhouseIdsSpec(DateOnly date, Guid[] henhouseIds)
    {
        Query.Where(fs => fs.Date == date && henhouseIds.Contains(fs.HenhouseId));
    }
}

public sealed class GetFallenStockByDateSpec : BaseSpecification<FallenStockEntity>,
    ISingleResultSpecification<FallenStockEntity>
{
    public GetFallenStockByDateSpec(Guid farmId, Guid cycleId, DateOnly date)
    {
        EnsureExists();
        DisableTracking();
        Query.Where(fs => fs.FarmId == farmId && fs.CycleId == cycleId && fs.Date == date)
            .Take(1);
    }
}