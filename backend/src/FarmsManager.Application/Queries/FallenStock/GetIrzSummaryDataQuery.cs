using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Services;
using FarmsManager.Application.Specifications;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Entities;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FarmsManager.Shared.Extensions;
using MediatR;

namespace FarmsManager.Application.Queries.FallenStock;

public record GetIrzSummaryDataQueryResponse
{
    public int CurrentStockSize { get; init; }
    public int ReportedFallenStock { get; init; }
    public int CollectedFallenStock { get; init; }
}

public record GetIrzSummaryDataQuery : IRequest<BaseResponse<GetIrzSummaryDataQueryResponse>>
{
    public Guid FarmId { get; init; }
    public string Cycle { get; init; }
    public int CycleIdentifier => int.Parse(Cycle.Split('-')[0]);
    public int CycleYear => int.Parse(Cycle.Split('-')[1]);
}

public class
    GetIrzSummaryDataQueryHandler : IRequestHandler<GetIrzSummaryDataQuery,
    BaseResponse<GetIrzSummaryDataQueryResponse>>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;
    private readonly IIrzplusService _irzplusService;
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IFallenStockRepository _fallenStockRepository;

    public GetIrzSummaryDataQueryHandler(IUserDataResolver userDataResolver, IUserRepository userRepository,
        IIrzplusService irzplusService, IFarmRepository farmRepository, ICycleRepository cycleRepository,
        IFallenStockRepository fallenStockRepository)
    {
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
        _irzplusService = irzplusService;
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
        _fallenStockRepository = fallenStockRepository;
    }

    public async Task<BaseResponse<GetIrzSummaryDataQueryResponse>> Handle(GetIrzSummaryDataQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.SingleOrDefaultAsync(new UserByIdSpec(userId), cancellationToken) ??
                   throw DomainException.UserNotFound();

        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.FarmId), cancellationToken);
        var cycle = await _cycleRepository.GetAsync(
            new GetCycleByYearIdentifierAndFarmSpec(request.FarmId, request.CycleYear, request.CycleIdentifier),
            cancellationToken);

        var fallenStocks = await _fallenStockRepository.ListAsync(
            new GetFallenStockByFarmAndCycleSentToIrzSpec(farm.Id, cycle.Id),
            cancellationToken);

        //TODO odbiory sztuk przez zakłady

        // _irzplusService.PrepareOptions(user.IrzplusCredentials);
        // var irzplusFlock = await _irzplusService.GetFlockAsync(farm, cancellationToken);

        var queryResponse = new GetIrzSummaryDataQueryResponse
        {
            CurrentStockSize = 0,//irzplusFlock.ListaDrob?.FirstOrDefault()?.OgolnaLiczbaDrobiu ?? 0,
            ReportedFallenStock = fallenStocks.Sum(t => t.Quantity),
            CollectedFallenStock = 0
        };

        var response = BaseResponse.CreateResponse(queryResponse);

        // if (irzplusFlock.Komunikat.IsNotEmpty())
        // {
        //     response.AddError("IRZplus", irzplusFlock.Komunikat);
        // }

        return response;
    }
}

public sealed class GetFallenStockByFarmAndCycleSentToIrzSpec : BaseSpecification<FallenStockEntity>
{
    public GetFallenStockByFarmAndCycleSentToIrzSpec(Guid farmId, Guid cycleId)
    {
        EnsureExists();
        DisableTracking();

        Query.Where(t => t.FarmId == farmId);
        Query.Where(t => t.CycleId == cycleId);
        Query.Where(t => t.DateIrzSentUtc.HasValue);
    }
}