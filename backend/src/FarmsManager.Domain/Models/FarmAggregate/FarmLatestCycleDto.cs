namespace FarmsManager.Domain.Models.FarmAggregate;

public record FarmLatestCycleDto
{
    public Guid Id { get; init; }
    public int Identifier { get; init; }
    public int Year { get; init; }
    public DateTime DateCreatedUtc { get; init; }
}