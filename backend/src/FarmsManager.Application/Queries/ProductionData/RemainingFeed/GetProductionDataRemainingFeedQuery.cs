using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models.ProductionData;
using MediatR;

namespace FarmsManager.Application.Queries.ProductionData.RemainingFeed;

public record GetProductionDataRemainingFeedQuery(ProductionDataQueryFilters Filters)
    : IRequest<BaseResponse<GetProductionDataRemainingFeedQueryResponse>>;