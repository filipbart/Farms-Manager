using FarmsManager.Application.Common.Responses;
using MediatR;

namespace FarmsManager.Application.Queries.FallenStock;

public record GetFallenStocksQuery : IRequest<BaseResponse<GetFallenStocksQueryResponse>>
{
    public Guid FarmId { get; init; }
    public string Cycle { get; init; }
    public int CycleIdentifier => int.Parse(Cycle.Split('-')[0]);
    public int CycleYear => int.Parse(Cycle.Split('-')[1]);
}