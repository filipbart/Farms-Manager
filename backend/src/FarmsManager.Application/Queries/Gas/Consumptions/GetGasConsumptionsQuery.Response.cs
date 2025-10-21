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
    public string CreatedByName { get; init; }
    public DateTime? DateModifiedUtc { get; init; }
    public string ModifiedByName { get; init; }
    public DateTime? DateDeletedUtc { get; init; }
    public string DeletedByName { get; init; }
}

public class GetGasConsumptionsQueryResponse : PaginationModel<GasConsumptionRowDto>;