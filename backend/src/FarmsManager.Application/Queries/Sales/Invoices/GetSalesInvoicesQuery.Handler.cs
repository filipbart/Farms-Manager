using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Sales.Invoices;

public class
    GetSalesInvoicesQueryHandler : IRequestHandler<GetSalesInvoicesQuery, BaseResponse<GetSalesInvoicesQueryResponse>>
{
    private readonly ISaleInvoiceRepository _saleInvoiceRepository;

    public GetSalesInvoicesQueryHandler(ISaleInvoiceRepository saleInvoiceRepository)
    {
        _saleInvoiceRepository = saleInvoiceRepository;
    }

    public async Task<BaseResponse<GetSalesInvoicesQueryResponse>> Handle(GetSalesInvoicesQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _saleInvoiceRepository.ListAsync<SalesInvoiceRowDto>(
            new GetAllSalesInvoicesSpec(request.Filters, true), cancellationToken);

        var count = await _saleInvoiceRepository.CountAsync(
            new GetAllSalesInvoicesSpec(request.Filters, false), cancellationToken);

        return BaseResponse.CreateResponse(new GetSalesInvoicesQueryResponse
        {
            TotalRows = count,
            Items = data
        });
    }
}

public class SalesInvoiceProfile : Profile
{
    public SalesInvoiceProfile()
    {
        CreateMap<SaleInvoiceEntity, SalesInvoiceRowDto>()
            .ForMember(m => m.CycleText, opt => opt.MapFrom(t => t.Cycle.Identifier + "/" + t.Cycle.Year))
            .ForMember(m => m.FarmName, opt => opt.MapFrom(t => t.Farm.Name))
            .ForMember(m => m.SlaughterhouseName, opt => opt.MapFrom(t => t.Slaughterhouse.Name));
    }
}