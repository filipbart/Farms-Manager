using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.FallenStocks;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
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
    private readonly IFallenStockPickupRepository _fallenStockPickupRepository;

    public GetIrzSummaryDataQueryHandler(IUserDataResolver userDataResolver, IUserRepository userRepository,
        IIrzplusService irzplusService, IFarmRepository farmRepository, ICycleRepository cycleRepository,
        IFallenStockRepository fallenStockRepository, IFallenStockPickupRepository fallenStockPickupRepository)
    {
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
        _irzplusService = irzplusService;
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
        _fallenStockRepository = fallenStockRepository;
        _fallenStockPickupRepository = fallenStockPickupRepository;
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

        var fallenStocks = await _fallenStockRepository.ListAsync(new FallenStockByFarmAndCycleSpec(farm.Id, cycle.Id),
            cancellationToken);

        var fallenStockPickups =
            await _fallenStockPickupRepository.ListAsync(new FallenStockPickupsByFarmAndCycleSpec(farm.Id, cycle.Id),
                cancellationToken);

        // _irzplusService.PrepareOptions(user.IrzplusCredentials);
        // var irzplusFlock = await _irzplusService.GetFlockAsync(farm, cancellationToken);

        var queryResponse = new GetIrzSummaryDataQueryResponse
        {
            CurrentStockSize = 0, //irzplusFlock.ListaDrob?.FirstOrDefault()?.OgolnaLiczbaDrobiu ?? 0,
            ReportedFallenStock = fallenStocks.Sum(t => t.Quantity),
            CollectedFallenStock = fallenStockPickups.Sum(t => t.Quantity)
        };

        var response = BaseResponse.CreateResponse(queryResponse);

        // if (irzplusFlock.Komunikat.IsNotEmpty())
        // {
        //     response.AddError("IRZplus", irzplusFlock.Komunikat);
        // }

        return response;
    }
}