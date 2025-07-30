using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.ProductionData.Weighings;

public record GetWeightStandardsQuery : IRequest<BaseResponse<GetWeightStandardsQueryResponse>>;

public record GetWeightStandardsQueryResponse(List<WeightStandardRowDto> Items);

public class WeightStandardRowDto
{
    public Guid Id { get; init; }
    public int Day { get; init; }
    public decimal Weight { get; init; }
}

public class
    GetWeightStandardsQueryHandler : IRequestHandler<GetWeightStandardsQuery,
    BaseResponse<GetWeightStandardsQueryResponse>>
{
    private readonly IProductionDataWeightStandardRepository _repository;

    public GetWeightStandardsQueryHandler(IProductionDataWeightStandardRepository repository)
    {
        _repository = repository;
    }

    public async Task<BaseResponse<GetWeightStandardsQueryResponse>> Handle(GetWeightStandardsQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _repository.ListAsync<WeightStandardRowDto>(new GetAllWeightStandardsSpec(),
            cancellationToken);

        var sortedData = data.OrderBy(s => s.Day).ToList();

        return BaseResponse.CreateResponse(new GetWeightStandardsQueryResponse(sortedData));
    }
}

public sealed class GetAllWeightStandardsSpec : BaseSpecification<ProductionDataWeightStandardEntity>
{
    public GetAllWeightStandardsSpec()
    {
        EnsureExists();
        DisableTracking();
    }
}

public class WeightStandardProfile : Profile
{
    public WeightStandardProfile()
    {
        CreateMap<ProductionDataWeightStandardEntity, WeightStandardRowDto>();
    }
}