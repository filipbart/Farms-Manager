using FarmsManager.Application.Common;

namespace FarmsManager.Application.Queries.ProductionData.RemainingFeed;

public class ProductionDataRemainingFeedRowDto
{
    public Guid Id { get; init; }
    public string CycleText { get; init; }
    public string FarmName { get; init; }
    public string HenhouseName { get; init; }
    public string FeedName { get; init; }
    public int RemainingTonnage { get; init; }
    public decimal RemainingValue { get; init; }
    public DateTime DateCreatedUtc { get; init; }
}

public class GetProductionDataRemainingFeedQueryResponse : PaginationModel<ProductionDataRemainingFeedRowDto>;