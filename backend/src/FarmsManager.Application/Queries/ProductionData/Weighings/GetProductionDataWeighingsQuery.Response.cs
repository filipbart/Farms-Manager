using FarmsManager.Application.Common;

namespace FarmsManager.Application.Queries.ProductionData.Weighings;

public class ProductionDataWeighingRowDto
{
    public Guid Id { get; init; }
    public DateTime DateCreatedUtc { get; init; }

    public string CycleText { get; init; }
    public string FarmName { get; init; }
    public string HenhouseName { get; init; }
    public string HatcheryName { get; init; }

    public int? Weighing1Day { get; init; }
    public decimal? Weighing1Weight { get; init; }
    public decimal? Weighing1Deviation { get; set; }

    public int? Weighing2Day { get; init; }
    public decimal? Weighing2Weight { get; init; }
    public decimal? Weighing2Deviation { get; set; }

    public int? Weighing3Day { get; init; }
    public decimal? Weighing3Weight { get; init; }
    public decimal? Weighing3Deviation { get; set; }

    public int? Weighing4Day { get; init; }
    public decimal? Weighing4Weight { get; init; }
    public decimal? Weighing4Deviation { get; set; }

    public int? Weighing5Day { get; init; }
    public decimal? Weighing5Weight { get; init; }
    public decimal? Weighing5Deviation { get; set; }
}

public class GetProductionDataWeighingsQueryResponse : PaginationModel<ProductionDataWeighingRowDto>;