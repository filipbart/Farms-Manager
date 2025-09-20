namespace FarmsManager.Application.Queries.FallenStockPickups;

public record FallenStockPickupsRowDto
{
    public Guid Id { get; init; }
    public Guid FarmId { get; init; }
    public string FarmName { get; init; }
    public Guid CycleId { get; init; }
    public string CycleText { get; init; }
    public DateOnly Date { get; init; }
    public int Quantity { get; init; }
    public DateTime DateCreatedUtc { get; init; }
}

public class GetFallenStockPickupsQueryResponse
{
    public List<FallenStockPickupsRowDto> Items { get; init; }
}