using FarmsManager.Application.Common;

namespace FarmsManager.Application.Models.ProductionData;

public enum ProductionDataOrderBy
{
    DateCreatedUtc,
    Cycle,
    Farm,
    Henhouse,
    DeadCount,
    DefectiveCount
}

public record ProductionDataQueryFilters : OrderedPaginationParams<ProductionDataOrderBy>
{
    public List<Guid> FarmIds { get; init; }
    public List<Guid> HenhouseIds { get; init; }
    public List<CycleDictModel> Cycles { get; init; }
    public DateOnly? DateSince { get; init; }
    public DateOnly? DateTo { get; init; }
}