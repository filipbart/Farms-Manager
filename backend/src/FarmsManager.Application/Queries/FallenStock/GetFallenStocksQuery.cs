using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models;
using MediatR;

namespace FarmsManager.Application.Queries.FallenStock;

public record GetFallenStocksQuery(Guid FarmId, Guid CycleId)
    : IRequest<BaseResponse<GetFallenStocksQueryResponse>>;