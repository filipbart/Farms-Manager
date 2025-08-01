using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Gas.Consumptions;

public class GetGasConsumptionsQueryHandler : IRequestHandler<GetGasConsumptionsQuery,
    BaseResponse<GetGasConsumptionsQueryResponse>>
{
    private readonly IGasConsumptionRepository _gasConsumptionRepository;

    public GetGasConsumptionsQueryHandler(IGasConsumptionRepository gasConsumptionRepository)
    {
        _gasConsumptionRepository = gasConsumptionRepository;
    }

    public async Task<BaseResponse<GetGasConsumptionsQueryResponse>> Handle(GetGasConsumptionsQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _gasConsumptionRepository.ListAsync<GasConsumptionRowDto>(
            new GetAllGasConsumptionsSpec(request.Filters, true), cancellationToken);

        var count = await _gasConsumptionRepository.CountAsync(
            new GetAllGasConsumptionsSpec(request.Filters, false), cancellationToken);

        return BaseResponse.CreateResponse(new GetGasConsumptionsQueryResponse
        {
            TotalRows = count,
            Items = data
        });
    }
}

public class GasConsumptionProfile : Profile
{
    public GasConsumptionProfile()
    {
        CreateMap<GasConsumptionEntity, GasConsumptionRowDto>()
            .ForMember(m => m.FarmName, opt => opt.MapFrom(t => t.Farm.Name))
            .ForMember(m => m.CycleText, opt => opt.MapFrom(t => t.Cycle.Identifier + "/" + t.Cycle.Year));
    }
}