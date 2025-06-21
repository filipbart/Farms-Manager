namespace FarmsManager.Domain.Models.FarmAggregate;

public record HenhouseRowDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public int Area { get; init; }
    public string Description { get; init; }
    public DateTime DateCreatedUtc { get; init; }
}