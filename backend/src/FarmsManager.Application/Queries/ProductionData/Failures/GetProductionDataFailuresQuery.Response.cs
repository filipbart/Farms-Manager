﻿using FarmsManager.Application.Common;

namespace FarmsManager.Application.Queries.ProductionData.Failures;

public class ProductionDataFailureRowDto
{
    public Guid Id { get; init; }
    public string CycleText { get; init; }
    public string FarmName { get; init; }
    public string HenhouseName { get; init; }
    public int DeadCount { get; init; }
    public int DefectiveCount { get; init; }
    public DateTime DateCreatedUtc { get; init; }
}

public class GetProductionDataFailuresQueryResponse : PaginationModel<ProductionDataFailureRowDto>;