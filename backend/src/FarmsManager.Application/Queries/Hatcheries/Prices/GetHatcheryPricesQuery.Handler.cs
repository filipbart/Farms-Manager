using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Hatcheries.Prices;

public class GetHatcheryPricesQueryHandler : IRequestHandler<GetHatcheryPricesQuery,
    BaseResponse<GetHatcheryPricesQueryResponse>>
{
    private readonly IHatcheryPriceRepository _hatcheryPriceRepository;

    public GetHatcheryPricesQueryHandler(IHatcheryPriceRepository hatcheryPriceRepository)
    {
        _hatcheryPriceRepository = hatcheryPriceRepository;
    }

    public async Task<BaseResponse<GetHatcheryPricesQueryResponse>> Handle(GetHatcheryPricesQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _hatcheryPriceRepository.ListAsync<HatcheryPriceRowDto>(
            new GetAllHatcheryPricesSpec(request.Filters, true), cancellationToken);

        var count = await _hatcheryPriceRepository.CountAsync(
            new GetAllHatcheryPricesSpec(request.Filters, false), cancellationToken);

        return BaseResponse.CreateResponse(new GetHatcheryPricesQueryResponse
        {
            TotalRows = count,
            Items = data
        });
    }
}

public class HatcheryPriceProfile : Profile
{
    public HatcheryPriceProfile()
    {
        CreateMap<HatcheryPriceEntity, HatcheryPriceRowDto>();
    }
}