using FarmsManager.Application.Common;

namespace FarmsManager.Application.Queries.Hatcheries.Prices;

public class HatcheryPriceRowDto
{
    public Guid Id { get; init; }
    public string HatcheryName { get; init; }
    public decimal Price { get; init; }
    public DateOnly Date { get; init; }
    public string Comment { get; init; }
    public DateTime DateCreatedUtc { get; init; }
}

public class GetHatcheryPricesQueryResponse : PaginationModel<HatcheryPriceRowDto>;