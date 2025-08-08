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
    public List<string> Cycles { get; init; }

    public List<CycleDictModel> CyclesDict => Cycles?.Select(c => new CycleDictModel
    {
        Identifier = int.Parse(c.Split('-')[0]),
        Year = int.Parse(c.Split('-')[1])
    }).ToList();

    public DateOnly? DateSince { get; init; }
    public DateOnly? DateTo { get; init; }
}