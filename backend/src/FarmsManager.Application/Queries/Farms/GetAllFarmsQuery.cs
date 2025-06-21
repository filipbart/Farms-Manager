using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Models.FarmAggregate;
using MediatR;

namespace FarmsManager.Application.Queries.Farms;

public record GetAllFarmsQuery : IRequest<BaseResponse<GetAllFarmsQueryResponse>>;

public class GetAllFarmsQueryResponse : PaginationModel<FarmRowDto>;

public class GetAllFarmsQueryHandler : IRequestHandler<GetAllFarmsQuery, BaseResponse<GetAllFarmsQueryResponse>>
{
    private readonly IFarmRepository _farmRepository;

    public GetAllFarmsQueryHandler(IFarmRepository farmRepository)
    {
        _farmRepository = farmRepository;
    }

    public async Task<BaseResponse<GetAllFarmsQueryResponse>> Handle(GetAllFarmsQuery request,
        CancellationToken cancellationToken)
    {
        var items = await _farmRepository.ListAsync<FarmRowDto>(new GetAllFarmsSpec(), cancellationToken);
        return BaseResponse.CreateResponse(new GetAllFarmsQueryResponse
        {
            TotalRows = items.Count,
            Items = items
        });
    }
}

public sealed class GetAllFarmsSpec : BaseSpecification<FarmEntity>
{
    public GetAllFarmsSpec()
    {
        EnsureExists();
    }
}