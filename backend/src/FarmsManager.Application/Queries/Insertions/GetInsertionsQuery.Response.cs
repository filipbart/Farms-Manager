using FarmsManager.Application.Common;

namespace FarmsManager.Application.Queries.Insertions;

public class InsertionRowDto
{
    public Guid Id { get; init; }
    public Guid CycleId { get; init; }
    public string CycleText { get; init; }
    public Guid FarmId { get; init; }
    public string FarmName { get; init; }
    public string HenhouseName { get; init; }
    public DateOnly InsertionDate { get; init; }
    public int Quantity { get; init; }
    public string HatcheryName { get; init; }
    public decimal BodyWeight { get; init; }
    public DateTime DateCreatedUtc { get; init; }
    public Guid InternalGroupId { get; init; }
    public DateTime? DateIrzSentUtc { get; init; }
    public bool IsSentToIrz { get; init; }
    public string DocumentNumber { get; init; }
    public string IrzComment { get; init; }
    public bool ReportedToWios { get; init; }
    public string WiosComment { get; init; }
    public string Comment { get; init; }
    public string CreatedByName { get; init; }
    public DateTime? DateModifiedUtc { get; init; }
    public string ModifiedByName { get; init; }
    public DateTime? DateDeletedUtc { get; init; }
    public string DeletedByName { get; init; }
}

public class GetInsertionsQueryResponse : PaginationModel<InsertionRowDto>;