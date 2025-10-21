using FarmsManager.Application.Common.Responses;
using MediatR;

namespace FarmsManager.Application.Queries.FallenStockPickups;

public record GetFallenStockPickupsQuery : IRequest<BaseResponse<GetFallenStockPickupsQueryResponse>>
{
    public Guid FarmId { get; init; }
    public string Cycle { get; init; }
    public bool? ShowDeleted { get; init; }
    public int CycleIdentifier => int.Parse(Cycle.Split('-')[0]);
    public int CycleYear => int.Parse(Cycle.Split('-')[1]);
}