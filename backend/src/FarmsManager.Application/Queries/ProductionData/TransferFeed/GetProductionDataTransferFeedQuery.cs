using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models.ProductionData;
using MediatR;

namespace FarmsManager.Application.Queries.ProductionData.TransferFeed;

public record GetProductionDataTransferFeedQuery(ProductionDataQueryFilters Filters)
    : IRequest<BaseResponse<GetProductionDataTransferFeedQueryResponse>>;