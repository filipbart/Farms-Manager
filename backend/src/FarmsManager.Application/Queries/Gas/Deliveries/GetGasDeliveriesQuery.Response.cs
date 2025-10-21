using FarmsManager.Application.Common;

namespace FarmsManager.Application.Queries.Gas.Deliveries;

public class GasDeliveryRowDto
{
    public Guid Id { get; init; }
    public DateTime DateCreatedUtc { get; init; }

    public string FarmName { get; init; }
    public string ContractorName { get; init; }
    public string InvoiceNumber { get; init; }
    public decimal UsedQuantity { get; init; }
    public DateOnly InvoiceDate { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal Quantity { get; init; }
    public string Comment { get; init; }
    public string FilePath { get; init; }
    public string CreatedByName { get; init; }
    public DateTime? DateModifiedUtc { get; init; }
    public string ModifiedByName { get; init; }
    public DateTime? DateDeletedUtc { get; init; }
    public string DeletedByName { get; init; }
}

public class GetGasDeliveriesQueryResponse : PaginationModel<GasDeliveryRowDto>;