using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.SeedWork.Entities;
using FarmsManager.Domain.Aggregates.SeedWork.Enums;
using FarmsManager.Domain.Aggregates.SeedWork.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.ColumnsViews;

public record GetColumnsViewsQueryResponse
{
    public List<ColumnViewEntity> Items { get; init; } = [];
}

public record GetColumnsViewsQuery(ColumnViewType Type) : IRequest<BaseResponse<GetColumnsViewsQueryResponse>>;

public class
    GetColumnsViewsQueryHandler : IRequestHandler<GetColumnsViewsQuery, BaseResponse<GetColumnsViewsQueryResponse>>
{
    private readonly IColumnViewRepository _columnViewRepository;

    public GetColumnsViewsQueryHandler(IColumnViewRepository columnViewRepository)
    {
        _columnViewRepository = columnViewRepository;
    }

    public async Task<BaseResponse<GetColumnsViewsQueryResponse>> Handle(GetColumnsViewsQuery request,
        CancellationToken cancellationToken)
    {
        var items = await _columnViewRepository.ListAsync(new GetColumnsViewsSpec(request.Type),
            cancellationToken);
        return BaseResponse.CreateResponse(new GetColumnsViewsQueryResponse
        {
            Items = items
        });
    }
}

public sealed class GetColumnsViewsSpec : BaseSpecification<ColumnViewEntity>
{
    public GetColumnsViewsSpec(ColumnViewType type)
    {
        EnsureExists();
        DisableTracking();
        Query.Where(t => t.Type == type);
    }
}