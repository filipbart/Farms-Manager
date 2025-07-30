using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models.ProductionData;
using MediatR;

namespace FarmsManager.Application.Queries.ProductionData.Weighings;

public record GetProductionDataWeighingsQuery(ProductionDataQueryFilters Filters)
    : IRequest<BaseResponse<GetProductionDataWeighingsQueryResponse>>;