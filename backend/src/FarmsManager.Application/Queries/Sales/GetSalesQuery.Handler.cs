using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Sales;

public class GetSalesQueryHandler : IRequestHandler<GetSalesQuery, BaseResponse<GetSalesQueryResponse>>
{
    private readonly ISaleRepository _saleRepository;

    public GetSalesQueryHandler(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<BaseResponse<GetSalesQueryResponse>> Handle(GetSalesQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _saleRepository.ListAsync<SaleRowDto>(
            new GetAllSalesSpec(request.Filters, true), cancellationToken);
        var count = await _saleRepository.CountAsync(new GetAllSalesSpec(request.Filters, false),
            cancellationToken);
        return BaseResponse.CreateResponse(new GetSalesQueryResponse
        {
            TotalRows = count,
            Items = data
        });
    }
}

public class SaleProfile : Profile
{
    public SaleProfile()
    {
        CreateMap<SaleEntity, SaleRowDto>()
            .ForMember(t => t.CycleText, opt => opt.MapFrom(t => t.Cycle.Identifier + "/" + t.Cycle.Year))
            .ForMember(t => t.FarmName, opt => opt.MapFrom(t => t.Farm.Name))
            .ForMember(t => t.HenhouseName, opt => opt.MapFrom(t => t.Henhouse.Name))
            .ForMember(t => t.SlaughterhouseName, opt => opt.MapFrom(t => t.Slaughterhouse.Name));
    }
}