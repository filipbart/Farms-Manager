using Ardalis.Specification;
using FarmsManager.Application.Commands.Insertions;
using FarmsManager.Application.Commands.UtilizationPlants;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Application.Specifications.Henhouses;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Entities;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Enums;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FarmsManager.Shared.Extensions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.FallenStocks;

public record AddNewFallenStocksCommand : IRequest<EmptyBaseResponse>
{
    public Guid FarmId { get; set; }
    public Guid CycleId { get; set; }
    public FallenStockType Type { get; set; }
    public Guid? UtilizationPlantId { get; set; }
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
    private readonly IInsertionRepository _insertionRepository;


    public AddNewFallenStocksCommandHandler(IUserDataResolver userDataResolver, IUserRepository userRepository,
        IFarmRepository farmRepository, ICycleRepository cycleRepository, IFallenStockRepository fallenStockRepository,
        IHenhouseRepository henhouseRepository, IUtilizationPlantRepository utilizationPlantRepository,
        IIrzplusService irzplusService, IInsertionRepository insertionRepository)
    {
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
        _fallenStockRepository = fallenStockRepository;
        _henhouseRepository = henhouseRepository;
        _utilizationPlantRepository = utilizationPlantRepository;
        _irzplusService = irzplusService;
        _insertionRepository = insertionRepository;
    }

    public async Task<EmptyBaseResponse> Handle(AddNewFallenStocksCommand request, CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.SingleOrDefaultAsync(new UserByIdSpec(userId), ct) ??
                   throw DomainException.UserNotFound();
        var response = new EmptyBaseResponse();

        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.FarmId), ct);
        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(request.CycleId), ct);

        UtilizationPlantEntity utilizationPlant = null;
        if (request.Type == FallenStockType.FallCollision && request.UtilizationPlantId.HasValue)
        {
            utilizationPlant =
                await _utilizationPlantRepository.GetAsync(
                    new UtilizationPlantByIdSpec(request.UtilizationPlantId.Value),
                    ct);
        }

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

        var allHenhouses = await _henhouseRepository.ListAsync(new HenhousesByIdsSpec(henhouseIds), ct);

        var existingFallenStocks =
            await _fallenStockRepository.ListAsync(
                new GetFallenStockByDateAndHenhouseIdsSpec(request.Date, henhouseIds), ct);

        var insertions =
            await _insertionRepository.ListAsync(
                new GetInsertionsByFarmCycleAndHenhouseIdsSpec(farm.Id, cycle.Id, henhouseIds), ct);

        var henhousesDict = allHenhouses.ToDictionary(h => h.Id);
        var existingFallenStockHenhouseIds = existingFallenStocks.Select(fs => fs.HenhouseId).ToHashSet();
        var insertionHenhouseIds = insertions.Select(i => i.HenhouseId).ToHashSet();

        foreach (var henhouseId in henhouseIds)
        {
            if (!henhousesDict.TryGetValue(henhouseId, out var henhouse))
            {
                response.AddError("Entries", $"Kurnik o ID '{henhouseId}' nie został znaleziony.");
                continue;
            }

            if (existingFallenStockHenhouseIds.Contains(henhouseId))
            {
                response.AddError("Entries", $"Kurnik '{henhouse.Name}' ma już zgłoszenie w tym dniu.");
            }

            if (!insertionHenhouseIds.Contains(henhouseId))
            {
                response.AddError("Insertions", $"Kurnik '{henhouse.Name}' nie ma wstawienia w danym cyklu.");
            }
        }

        if (response.Errors.Count != 0)
        {
            return response;
        }

        var existingGroupForDate = await _fallenStockRepository.FirstOrDefaultAsync(
            new GetFallenStockByDateSpec(request.FarmId, request.CycleId, request.Date), ct);
        if (existingGroupForDate is not null && existingGroupForDate.Type != request.Type)
        {
            response.AddError("Type",
                $"Dla wybranej daty istnieje już zgłoszenie innego typu ({existingGroupForDate.Type}). Wszystkie zgłoszenia w danym dniu muszą być tego samego typu.");
            return response;
        }

        var internalGroupId = existingGroupForDate?.InternalGroupId ?? Guid.NewGuid();

        var newFallenStocks = new List<FallenStockEntity>();
        foreach (var entry in request.Entries)
        {
            var henhouse = henhousesDict[entry.HenhouseId];

            var newFallenStock = FallenStockEntity.CreateNew(
                internalGroupId,
                farm,
                cycle,
                utilizationPlant,
                henhouse,
                request.Type,
                request.Date,
                entry.Quantity,
                userId
            );
            newFallenStocks.Add(newFallenStock);
        }

        if (newFallenStocks.Count == 0)
        {
            return response;
        }

        if (request.SendToIrz)
        {
            var irzplusCredential = user.IrzplusCredentials?.FirstOrDefault(t => t.FarmId == farm.Id);
            if (irzplusCredential is null)
            {
                response.AddError("IrzplusCredentials", "Brak danych logowania do systemu IRZplus");
                return response;
            }

            _irzplusService.PrepareOptions(irzplusCredential);
            var dispositionResponse = await _irzplusService.SendFallenStocksAsync(newFallenStocks, ct);
            if (dispositionResponse.Bledy.Count != 0)
            {
                foreach (var bladWalidacjiDto in dispositionResponse.Bledy)
                {
                    response.AddError(bladWalidacjiDto.KodBledu, bladWalidacjiDto.Komunikat);
                }

                return response;
            }

            if (dispositionResponse.NumerDokumentu.IsEmpty())
            {
                throw new Exception("Numer dokumentu z systemu IRZplus jest pusty");
            }

            foreach (var fallenStockEntity in newFallenStocks)
            {
                fallenStockEntity.MarkAsSentToIrz(dispositionResponse.NumerDokumentu, userId);
            }
        }

        await _fallenStockRepository.AddRangeAsync(newFallenStocks, ct);

        return response;
    }
}

public class AddNewFallenStocksValidator : AbstractValidator<AddNewFallenStocksCommand>
{
    public AddNewFallenStocksValidator()
    {
        RuleFor(x => x.FarmId).NotEmpty();
        RuleFor(x => x.CycleId).NotEmpty();
        RuleFor(x => x.UtilizationPlantId).NotEmpty().When(t => t.Type == FallenStockType.FallCollision);
        RuleFor(x => x.UtilizationPlantId).Empty().When(t => t.Type == FallenStockType.EndCycle);
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
        EnsureExists();
        DisableTracking();
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