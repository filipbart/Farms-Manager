using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.SalesSettings;

public record GetSaleFieldsExtraQuery : IRequest<BaseResponse<GetSaleFieldsExtraQueryResponse>>;

public record GetSaleFieldsExtraQueryResponse
{
    public List<SaleFieldExtraRow> Fields { get; init; }
}

public record SaleFieldExtraRow
{
    public Guid Id { get; init; }
    public string Name { get; init; }
}

public class
    GetSaleFieldsExtraQueryHandler : IRequestHandler<GetSaleFieldsExtraQuery,
    BaseResponse<GetSaleFieldsExtraQueryResponse>>
{
    private readonly ISaleFieldExtraRepository _saleFieldExtraRepository;

    public GetSaleFieldsExtraQueryHandler(ISaleFieldExtraRepository saleFieldExtraRepository)
    {
        _saleFieldExtraRepository = saleFieldExtraRepository;
    }

    public async Task<BaseResponse<GetSaleFieldsExtraQueryResponse>> Handle(GetSaleFieldsExtraQuery request,
        CancellationToken cancellationToken)
    {
        var items = await _saleFieldExtraRepository.ListAsync<SaleFieldExtraRow>(new GetAllSaleFieldsExtraSpec(),
            cancellationToken);
        return BaseResponse.CreateResponse(new GetSaleFieldsExtraQueryResponse
        {
            Fields = items
        });
    }
}

public sealed class GetAllSaleFieldsExtraSpec : BaseSpecification<SaleFieldExtraEntity>
{
    public GetAllSaleFieldsExtraSpec()
    {
        EnsureExists();
    }
}

public class SaleFieldsExtraProfile : Profile
{
    public SaleFieldsExtraProfile()
    {
        CreateMap<SaleFieldExtraEntity, SaleFieldExtraRow>();
    }
}