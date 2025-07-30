using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.ProductionData.Weighings;

public record GetHatcheryForWeighingQueryResponse
{
    public Guid? HatcheryId { get; init; }
}

public record GetHatcheryForWeighingQuery : IRequest<BaseResponse<GetHatcheryForWeighingQueryResponse>>
{
    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public Guid HenhouseId { get; init; }
}

public class
    GetHatcheryForWeighingQueryHandler : IRequestHandler<GetHatcheryForWeighingQuery,
    BaseResponse<GetHatcheryForWeighingQueryResponse>>
{
    private readonly IInsertionRepository _insertionRepository;

    public GetHatcheryForWeighingQueryHandler(IInsertionRepository insertionRepository)
    {
        _insertionRepository = insertionRepository;
    }

    public async Task<BaseResponse<GetHatcheryForWeighingQueryResponse>> Handle(GetHatcheryForWeighingQuery request,
        CancellationToken cancellationToken)
    {
        var insertion = await _insertionRepository.FirstOrDefaultAsync(
            new GetHatcheryForWeighingSpec(request.FarmId, request.CycleId, request.HenhouseId), cancellationToken);

        return BaseResponse.CreateResponse(new GetHatcheryForWeighingQueryResponse
        {
            HatcheryId = insertion?.HatcheryId
        });
    }
}

public sealed class GetHatcheryForWeighingSpec : BaseSpecification<InsertionEntity>
{
    public GetHatcheryForWeighingSpec(Guid farmId, Guid cycleId, Guid henhouseId)
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