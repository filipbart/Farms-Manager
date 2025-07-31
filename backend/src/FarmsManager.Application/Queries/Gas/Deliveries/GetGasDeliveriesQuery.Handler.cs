using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Gas.Deliveries;

public class GetGasDeliveriesQueryHandler : IRequestHandler<GetGasDeliveriesQuery,
    BaseResponse<GetGasDeliveriesQueryResponse>>
{
    private readonly IGasDeliveryRepository _gasDeliveryRepository;

    public GetGasDeliveriesQueryHandler(IGasDeliveryRepository gasDeliveryRepository)
    {
        _gasDeliveryRepository = gasDeliveryRepository;
    }

    public async Task<BaseResponse<GetGasDeliveriesQueryResponse>> Handle(GetGasDeliveriesQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _gasDeliveryRepository.ListAsync<GasDeliveryRowDto>(
            new GetAllGasDeliveriesSpec(request.Filters, true), cancellationToken);

        var count = await _gasDeliveryRepository.CountAsync(
            new GetAllGasDeliveriesSpec(request.Filters, false), cancellationToken);

        return BaseResponse.CreateResponse(new GetGasDeliveriesQueryResponse
        {
            TotalRows = count,
            Items = data
        });
    }
}

public class GasDeliveryProfile : Profile
{
    public GasDeliveryProfile()
    {
        CreateMap<GasDeliveryEntity, GasDeliveryRowDto>()
            .ForMember(m => m.FarmName, opt => opt.MapFrom(t => t.Farm.Name))
            .ForMember(m => m.ContractorName, opt => opt.MapFrom(t => t.GasContractor.Name));
    }
}