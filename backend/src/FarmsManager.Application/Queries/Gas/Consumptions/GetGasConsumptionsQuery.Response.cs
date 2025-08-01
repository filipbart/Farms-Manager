using FarmsManager.Application.Common;

namespace FarmsManager.Application.Queries.Gas.Consumptions;

public class GasConsumptionRowDto
{
    public Guid Id { get; init; }
    public DateTime DateCreatedUtc { get; init; }
    public Guid FarmId { get; init; }
    public string CycleText { get; init; }
    public string FarmName { get; init; }
    public decimal QuantityConsumed { get; init; }
    public decimal Cost { get; init; }
}

public class GetGasConsumptionsQueryResponse : PaginationModel<GasConsumptionRowDto>;