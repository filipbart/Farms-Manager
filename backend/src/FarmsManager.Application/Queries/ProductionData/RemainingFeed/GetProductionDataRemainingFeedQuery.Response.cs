using FarmsManager.Application.Common;

namespace FarmsManager.Application.Queries.ProductionData.RemainingFeed;

public class ProductionDataRemainingFeedRowDto
{
    public Guid Id { get; init; }
    public Guid CycleId { get; init; }
    public string CycleText { get; init; }
    public Guid FarmId { get; init; }
    public string FarmName { get; init; }
    public string HenhouseName { get; init; }
    public string FeedName { get; init; }
    public decimal RemainingTonnage { get; init; }
    public decimal RemainingValue { get; init; }
    public DateTime DateCreatedUtc { get; init; }
    public string CreatedByName { get; init; }
    public DateTime? DateModifiedUtc { get; init; }
    public string ModifiedByName { get; init; }
    public DateTime? DateDeletedUtc { get; init; }
    public string DeletedByName { get; init; }
}

public class GetProductionDataRemainingFeedQueryResponse : PaginationModel<ProductionDataRemainingFeedRowDto>;