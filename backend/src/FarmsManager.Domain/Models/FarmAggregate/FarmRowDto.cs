namespace FarmsManager.Domain.Models.FarmAggregate;

public record FarmRowDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Nip { get; init; }
    public string Address { get; init; }
    public int HenHousesCount { get; init; }
    public DateTime DateCreatedUtc { get; init; }
}