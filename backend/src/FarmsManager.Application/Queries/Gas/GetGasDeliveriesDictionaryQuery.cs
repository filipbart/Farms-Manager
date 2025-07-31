using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models;
using FarmsManager.Application.Queries.Farms;
using FarmsManager.Application.Specifications.Gas;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Gas;

public record GetGasDeliveriesDictionaryQueryResponse
{
    public List<FarmDictModel> Farms { get; set; } = [];
    public List<DictModel> Contractors { get; set; } = [];
}

public record GetGasDeliveriesDictionaryQuery : IRequest<BaseResponse<GetGasDeliveriesDictionaryQueryResponse>>;

public class GetGasDeliveriesDictionaryQueryHandler : IRequestHandler<GetGasDeliveriesDictionaryQuery,
    BaseResponse<GetGasDeliveriesDictionaryQueryResponse>>
{
    private readonly IFarmRepository _farmRepository;
    private readonly IGasContractorRepository _gasContractorRepository;

    public GetGasDeliveriesDictionaryQueryHandler(IFarmRepository farmRepository,
        IGasContractorRepository gasContractorRepository)
    {
        _farmRepository = farmRepository;
        _gasContractorRepository = gasContractorRepository;
    }

    public async Task<BaseResponse<GetGasDeliveriesDictionaryQueryResponse>> Handle(
        GetGasDeliveriesDictionaryQuery request, CancellationToken cancellationToken)
    {
        var farms = await _farmRepository.ListAsync<FarmDictModel>(new GetAllFarmsSpec(), cancellationToken);
        var contractors =
            await _gasContractorRepository.ListAsync<DictModel>(new GetAllGasContractorsSpec(), cancellationToken);

        return BaseResponse.CreateResponse(new GetGasDeliveriesDictionaryQueryResponse
        {
            Farms = farms,
            Contractors = contractors,
        });
    }
}

public class GasDeliveriesDictionaryProfile : Profile
{
    public GasDeliveriesDictionaryProfile()
    {
        CreateMap<GasContractorEntity, DictModel>();
    }
}