using FarmsManager.Application.Common;

namespace FarmsManager.Application.Queries.Insertions;

public class InsertionRowDto
{
    public Guid Id { get; init; }
    public string CycleText { get; init; }
    public string FarmName { get; init; }
    public string HenhouseName { get; init; }
    public DateOnly InsertionDate { get; init; }
    public int Quantity { get; init; }
    public string HatcheryName { get; init; }
    public decimal BodyWeight { get; init; }
    public DateTime DateCreatedUtc { get; init; }
}

public class GetInsertionsQueryResponse : PaginationModel<InsertionRowDto>;