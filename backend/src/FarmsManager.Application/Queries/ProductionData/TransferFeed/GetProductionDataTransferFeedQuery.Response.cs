using FarmsManager.Application.Common;

namespace FarmsManager.Application.Queries.ProductionData.TransferFeed;

public class ProductionDataTransferFeedRowDto
{
    public Guid Id { get; init; }
    public string FromCycleText { get; init; }
    public string FromFarmName { get; init; }
    public string FromHenhouseName { get; init; }
    public string ToCycleText { get; init; }
    public string ToFarmName { get; init; }
    public string ToHenhouseName { get; init; }
    public string FeedName { get; init; }
    public int RemainingTonnage { get; init; }
    public decimal RemainingValue { get; init; }
    public DateTime DateCreatedUtc { get; init; }
}

public class GetProductionDataTransferFeedQueryResponse : PaginationModel<ProductionDataTransferFeedRowDto>;