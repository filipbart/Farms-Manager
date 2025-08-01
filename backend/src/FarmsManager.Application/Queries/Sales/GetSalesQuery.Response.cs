﻿using FarmsManager.Application.Common;
using FarmsManager.Application.Extensions;
using FarmsManager.Domain.Aggregates.FarmAggregate.Enums;
using FarmsManager.Domain.Aggregates.FarmAggregate.Models;

namespace FarmsManager.Application.Queries.Sales;

public class SaleRowDto
{
    public Guid Id { get; init; }
    public string CycleText { get; init; }
    public string FarmName { get; init; }
    public string HenhouseName { get; init; }
    public string SlaughterhouseName { get; init; }
    public SaleType Type { get; init; }
    public string TypeDesc => Type.GetDescription();
    public DateOnly SaleDate { get; init; }
    public decimal Weight { get; init; }
    public int Quantity { get; init; }
    public decimal ConfiscatedWeight { get; init; }
    public int ConfiscatedCount { get; init; }
    public decimal DeadWeight { get; init; }
    public int DeadCount { get; init; }
    public decimal FarmerWeight { get; init; }
    public decimal BasePrice { get; init; }
    public decimal PriceWithExtras { get; init; }
    public List<SaleOtherExtras> OtherExtras { get; init; }
    public string Comment { get; init; }
    public DateTime DateCreatedUtc { get; init; }
    public Guid InternalGroupId { get; init; }
    public DateTime? DateIrzSentUtc { get; init; }
    public bool IsSentToIrz { get; init; }
    public string DocumentNumber { get; init; }
    public string DirectoryPath { get; init; }
}

public class GetSalesQueryResponse : PaginationModel<SaleRowDto>;