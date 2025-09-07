using Ardalis.Specification;
using AutoMapper;
using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Entities;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.UtilizationPlants;

public record UtilizationPlantRowDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Nip { get; init; }
    public string Address { get; init; }
    public string IrzNumber { get; init; }
    public DateTime DateCreatedUtc { get; init; }
}

public record GetAllUtilizationPlantsQuery : IRequest<BaseResponse<GetAllUtilizationPlantsQueryResponse>>;

public class GetAllUtilizationPlantsQueryResponse : PaginationModel<UtilizationPlantRowDto>;

public class
    GetAllUtilizationPlantsQueryHandler : IRequestHandler<GetAllUtilizationPlantsQuery,
    BaseResponse<GetAllUtilizationPlantsQueryResponse>>
{
    private readonly IUtilizationPlantRepository _utilizationPlantRepository;

    public GetAllUtilizationPlantsQueryHandler(IUtilizationPlantRepository utilizationPlantRepository)
    {
        _utilizationPlantRepository = utilizationPlantRepository;
    }

    public async Task<BaseResponse<GetAllUtilizationPlantsQueryResponse>> Handle(GetAllUtilizationPlantsQuery request,
        CancellationToken cancellationToken)
    {
        var items = await _utilizationPlantRepository.ListAsync<UtilizationPlantRowDto>(
            new GetAllUtilizationPlantsSpec(),
            cancellationToken);

        return BaseResponse.CreateResponse(new GetAllUtilizationPlantsQueryResponse
        {
            TotalRows = items.Count,
            Items = items
        });
    }
}

public sealed class GetAllUtilizationPlantsSpec : BaseSpecification<UtilizationPlantEntity>
{
    public GetAllUtilizationPlantsSpec()
    {
        EnsureExists();
        Query.OrderBy(t => t.Name);
    }
}

public class UtilizationPlantProfile : Profile
{
    public UtilizationPlantProfile()
    {
        CreateMap<UtilizationPlantEntity, UtilizationPlantRowDto>();
    }
}