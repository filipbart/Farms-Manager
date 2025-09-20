using FarmsManager.Application.Common;

namespace FarmsManager.Application.Queries.ProductionData.FlockLosses;

public class ProductionDataFlockLossRowDto
{
    public Guid Id { get; init; }
    public DateTime DateCreatedUtc { get; init; }

    public Guid CycleId { get; init; }
    public string CycleText { get; init; }
    public Guid FarmId { get; init; }
    public string FarmName { get; init; }
    public string HenhouseName { get; init; }
    public string HatcheryName { get; init; }
    public int InsertionQuantity { get; init; }

    public int? FlockLoss1Day { get; init; }
    public int? FlockLoss1Quantity { get; init; }
    public decimal? FlockLoss1Percentage { get; init; }

    public int? FlockLoss2Day { get; init; }
    public int? FlockLoss2Quantity { get; init; }
    public decimal? FlockLoss2Percentage { get; init; }

    public int? FlockLoss3Day { get; init; }
    public int? FlockLoss3Quantity { get; init; }
    public decimal? FlockLoss3Percentage { get; init; }

    public int? FlockLoss4Day { get; init; }
    public int? FlockLoss4Quantity { get; init; }
    public decimal? FlockLoss4Percentage { get; init; }
}

public class GetProductionDataFlockLossesQueryResponse : PaginationModel<ProductionDataFlockLossRowDto>;