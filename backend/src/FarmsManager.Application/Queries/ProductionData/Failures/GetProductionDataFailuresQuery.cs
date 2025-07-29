using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models.ProductionData;
using MediatR;

namespace FarmsManager.Application.Queries.ProductionData.Failures;

public record GetProductionDataFailuresQuery(ProductionDataQueryFilters Filters)
    : IRequest<BaseResponse<GetProductionDataFailuresQueryResponse>>;