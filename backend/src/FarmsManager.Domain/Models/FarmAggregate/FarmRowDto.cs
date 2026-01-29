namespace FarmsManager.Domain.Models.FarmAggregate;

public record FarmRowDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Nip { get; init; }
    public string Address { get; init; }
    public int HenHousesCount { get; init; }
    public string ProducerNumber { get; init; }
    public DateTime DateCreatedUtc { get; init; }
    public List<HenhouseRowDto> Henhouses { get; init; }
    public CycleDto ActiveCycle { get; init; }
    public Guid? TaxBusinessEntityId { get; init; }
    public string TaxBusinessEntityName { get; init; }
    public string CreatedByName { get; init; }
    public DateTime? DateModifiedUtc { get; init; }
    public string ModifiedByName { get; init; }
    public DateTime? DateDeletedUtc { get; init; }
    public string DeletedByName { get; init; }
}