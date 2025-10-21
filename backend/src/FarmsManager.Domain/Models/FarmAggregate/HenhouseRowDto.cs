namespace FarmsManager.Domain.Models.FarmAggregate;

public record HenhouseRowDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Code { get; init; }
    public int Area { get; init; }
    public string Description { get; init; }
    public DateTime DateCreatedUtc { get; init; }
    public string CreatedByName { get; init; }
    public DateTime? DateModifiedUtc { get; init; }
    public string ModifiedByName { get; init; }
    public DateTime? DateDeletedUtc { get; init; }
    public string DeletedByName { get; init; }
}